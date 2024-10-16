// <copyright file="MainWindowViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;

using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Interfaces;
using MigrationApp.GUI.Models;
using MigrationApp.GUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// ViewModel containing the logic bindings for the main application window.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private const string MigrationMessagesSessionSeperator = "--------------------------------------------";
    private readonly ITableauMigrationService migrationService;
    private bool isMigrating = false;
    private IProgressUpdater progressUpdater;
    private string notificationMessage = string.Empty;
    private string notificationDetailsMessage = string.Empty;
    private CancellationTokenSource? cancellationTokenSource = null;
    private IImmutableSolidColorBrush notificationColor = Brushes.Black;
    private ILogger<MainWindowViewModel>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    /// <param name="migrationService">The migration service.</param>
    /// <param name="emailDomainOptions">The default domain mapping to apply to users who do not have an existing email, or mapping present.</param>
    /// <param name="dictionaryUserMappingOptions">The user-specific mappings to be used if provided through CSV.</param>
    /// <param name="progressUpdater">The object to track the migration progress from the migration service.</param>
    /// <param name="publisher">The progress publisher for progress status messages.</param>
    /// <param name="filePicker">The file picker service to use for CSV loaded user mappings.</param>
    /// <param name="csvParser">The csv parser to load and parser user mappings.</param>
    public MainWindowViewModel(
        ITableauMigrationService migrationService,
        IOptions<EmailDomainMappingOptions> emailDomainOptions,
        IOptions<DictionaryUserMappingOptions> dictionaryUserMappingOptions,
        IProgressUpdater progressUpdater,
        IProgressMessagePublisher publisher,
        IFilePicker filePicker,
        ICsvParser csvParser)
    {
        this.migrationService = migrationService;

        // Sub View Models
        this.MessageDisplayVM = new MessageDisplayViewModel(publisher);
        this.ServerCredentialsVM = new AuthCredentialsViewModel(TableauEnv.TableauServer);
        this.CloudCredentialsVM = new AuthCredentialsViewModel(TableauEnv.TableauCloud);
        this.UserMappingsVM = new UserMappingsViewModel(
            emailDomainOptions,
            dictionaryUserMappingOptions,
            filePicker,
            csvParser);

        this.progressUpdater = progressUpdater;
        this.logger = App.ServiceProvider?.GetRequiredService<ILogger<MainWindowViewModel>>();

        // Subscribe to the progress updater event and retrigger UI rendering on update
        this.progressUpdater.OnProgressChanged += async (sender, args) =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                this.OnPropertyChanged(nameof(this.CurrentMigrationStateIndex));
                this.OnPropertyChanged(nameof(this.CurrentMigrationMessage));
            });
        };
    }

    /// <summary>
    /// Occurs when changes occur that affect the error state of the input fields.
    /// </summary>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Gets the total number of migration states available.
    /// </summary>
    public static int NumMigrationStates => ProgressUpdater.NumMigrationStates;

    /// <summary>
    /// Gets the progress status Message Display View Model.
    /// </summary>
    public MessageDisplayViewModel MessageDisplayVM { get; }

    /// <summary>
    /// Gets the ViewModel for the Tableau Server Credentials.
    /// </summary>
    public AuthCredentialsViewModel ServerCredentialsVM { get; }

    /// <summary>
    /// Gets the ViewModel for the Tableau cloud Credentials.
    /// </summary>
    public AuthCredentialsViewModel CloudCredentialsVM { get; }

    /// <summary>
    /// Gets the ViewModel for the Tableau Server to Cloud User Mappings.
    /// </summary>
    public UserMappingsViewModel UserMappingsVM { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not a migration is ongoing.
    /// </summary>
    public bool IsMigrating
    {
        get => this.isMigrating;
        set
        {
            if (this.isMigrating != value)
            {
                this.SetProperty(ref this.isMigrating, value);
            }
        }
    }

    /// <summary>
    /// Gets or Sets the notification message at the bottom of the main window.
    /// </summary>
    public string NotificationMessage
    {
        get => this.notificationMessage;
        set => this.SetProperty(ref this.notificationMessage, value);
    }

    /// <summary>
    /// Gets or Sets the notification message at the bottom of the main window.
    /// </summary>
    public string NotificationDetailsMessage
    {
        get => this.notificationDetailsMessage;
        set => this.SetProperty(ref this.notificationDetailsMessage, value);
    }

    /// <summary>
    /// Gets or Sets the color to be used for the notification message.
    /// </summary>
    public IImmutableSolidColorBrush NotificationColor
    {
        get => this.notificationColor;
        set => this.SetProperty(ref this.notificationColor, value);
    }

    /// <summary>
    /// Gets the current migration state index.
    /// </summary>
    public int CurrentMigrationStateIndex => this.progressUpdater.CurrentMigrationStateIndex;

    /// <summary>
    /// Gets the message associated with the current migration state.
    /// </summary>
    public string CurrentMigrationMessage => this.progressUpdater.CurrentMigrationMessage;

    /// <summary>
    /// Cancels the ongoing migration.
    /// </summary>
    public void CancelMigration()
    {
        this.cancellationTokenSource?.Cancel();
        return;
    }

    /// <summary>
    /// Callback to update fields when error state is changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnErrorsChanged(string propertyName)
    {
        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private async Task RunMigrationAsync()
    {
        var serverCreds = new EndpointOptions
        {
            Url = new Uri(this.ServerCredentialsVM.UriDetailsVM.UriBase),
            SiteContentUrl = this.ServerCredentialsVM.UriDetailsVM.SiteContent,
            AccessTokenName = this.ServerCredentialsVM.TokenDetailsVM.TokenName,
            AccessToken = this.ServerCredentialsVM.TokenDetailsVM.TokenSecret,
        };

        var cloudCreds = new EndpointOptions
        {
            Url = new Uri(this.CloudCredentialsVM.UriDetailsVM.UriBase),
            SiteContentUrl = this.CloudCredentialsVM.UriDetailsVM.SiteContent,
            AccessTokenName = this.CloudCredentialsVM.TokenDetailsVM.TokenName,
            AccessToken = this.CloudCredentialsVM.TokenDetailsVM.TokenSecret,
        };

        bool planBuilt = this.migrationService.BuildMigrationPlan(serverCreds, cloudCreds);

        if (planBuilt)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            DetailedMigrationResult migrationResult = await this.migrationService.StartMigrationTaskAsync(this.cancellationTokenSource.Token);

            if (migrationResult.status == ITableauMigrationService.MigrationStatus.SUCCESS)
            {
                this.NotificationMessage = "Migration Completed.";
                this.NotificationColor = Brushes.Green;
            }
            else if (migrationResult.status == ITableauMigrationService.MigrationStatus.CANCELLED)
            {
                this.NotificationMessage = "Migration Cancelled.";
                this.NotificationDetailsMessage = this.BuildErrorDetails(migrationResult.errors);
                this.NotificationColor = Brushes.Red;
            }
            else
            {
                this.NotificationMessage = "Migration Failed.";
                this.NotificationDetailsMessage = this.BuildErrorDetails(migrationResult.errors);
                this.NotificationColor = Brushes.Red;
            }
        }
        else
        {
            this.NotificationMessage = "Migration plan building failed.";
            this.NotificationColor = Brushes.Red;
        }

        this.MessageDisplayVM.AddMessage(MigrationMessagesSessionSeperator);
        this.IsMigrating = false;
        this.progressUpdater.Reset();
    }

    /// <summary>
    /// Helper method to build error details from exceptions.
    /// </summary>
    private string BuildErrorDetails(IReadOnlyList<Exception> errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return "No additional error information available.";
        }

        return string.Join(Environment.NewLine, errors.Select(e => e.Message));
    }

    [RelayCommand]
    private void RunMigration()
    {
        if (!this.AreFieldsValid())
        {
            this.logger?.LogInformation("Migration Run failed due to validation errors.");
            return;
        }

        this.IsMigrating = true;
        this.MessageDisplayVM.AddMessage(MigrationMessagesSessionSeperator);
        this.MessageDisplayVM.AddMessage("Migration Started");
        this.logger?.LogInformation("Migration Started");
        this.RunMigrationAsync().ConfigureAwait(false);
    }

    private bool AreFieldsValid()
    {
        this.ServerCredentialsVM.ValidateAll();
        this.CloudCredentialsVM.ValidateAll();
        this.UserMappingsVM.ValidateAll();

        int errorCount =
            this.ServerCredentialsVM.GetErrorCount()
            + this.CloudCredentialsVM.GetErrorCount()
            + this.UserMappingsVM.GetErrorCount();

        if (errorCount < 0)
        {
            this.logger?.LogError($"Validation encountered an unexpected error count: {errorCount}");
            throw new InvalidOperationException("Invalid validation error count.");
        }

        return errorCount == 0;
    }
}