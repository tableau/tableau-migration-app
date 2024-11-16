// <copyright file="MainWindowViewModel.cs" company="Salesforce, Inc.">
// Copyright (c) 2024, Salesforce, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2
//
// Licensed under the Apache License, Version 2.0 (the "License")
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Tableau.Migration.App.GUI.ViewModels;

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Tableau.Migration.App.GUI.Views;

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
    /// <param name="timerController">The object to track the migration timers from the migration service.</param>
    /// <param name="publisher">The progress publisher for progress status messages.</param>
    /// <param name="filePicker">The file picker service to use for CSV loaded user mappings.</param>
    /// <param name="csvParser">The csv parser to load and parser user mappings.</param>
    public MainWindowViewModel(
        ITableauMigrationService migrationService,
        IOptions<EmailDomainMappingOptions> emailDomainOptions,
        IOptions<DictionaryUserMappingOptions> dictionaryUserMappingOptions,
        IProgressUpdater progressUpdater,
        IProgressTimerController timerController,
        IProgressMessagePublisher publisher,
        IFilePicker filePicker,
        ICsvParser csvParser)
    {
        this.migrationService = migrationService;

        // Sub View Models
        this.MessageDisplayVM = new MessageDisplayViewModel(publisher);
        this.TimerDisplayVM = new TimerDisplayViewModel(timerController, progressUpdater);
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
    /// Gets the progress status Message Display View Model.
    /// </summary>
    public TimerDisplayViewModel TimerDisplayVM { get; }

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
    /// Saves the migration manifest if a save file path is provided.
    /// </summary>
    /// <param name="manifestSaveFilePath">The file path where the manifest should be saved, or null if saving is not required.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task SaveManifestIfRequiredAsync(string? manifestSaveFilePath)
    {
        if (!string.IsNullOrEmpty(manifestSaveFilePath))
        {
            bool isSaved = await this.migrationService.SaveManifestAsync(manifestSaveFilePath);
            this.NotificationMessage += isSaved ? " Manifest saved." : " Failed to save manifest.";
        }
        else
        {
            this.NotificationMessage += " Manifest was not saved.";
        }
    }

    /// <summary>
    /// Callback to update fields when error state is changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnErrorsChanged(string propertyName)
    {
        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Validates fields and starts the migration process if valid.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [RelayCommand]
    private async Task RunMigration()
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
        await this.RunMigrationTask().ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously resumes migration by selecting a manifest file and calling RunResumeMigration with the file path.
    /// </summary>
    [RelayCommand]
    private async Task ResumeMigration()
    {
        if (!this.AreFieldsValid())
        {
            this.logger?.LogInformation("Migration Run failed due to validation errors.");
            return;
        }

        var filePath = await this.SelectManifestFileAsync();
        if (filePath != null)
        {
            this.IsMigrating = true;
            this.MessageDisplayVM.AddMessage(MigrationMessagesSessionSeperator);
            this.MessageDisplayVM.AddMessage("Migration Started");
            this.logger?.LogInformation("Migration Started");
            await this.RunMigrationTask().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Opens a file picker dialog to select a manifest file and returns the selected file path.
    /// </summary>
    /// <returns>The file path if a file is selected; otherwise, <c>null</c>.</returns>
    private async Task<string?> SelectManifestFileAsync()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Select Manifest File",
            AllowMultiple = false, // Allow only one file selection
            FileTypeFilter = new List<FilePickerFileType>
        {
            new FilePickerFileType("JSON files") { Patterns = new[] { "*.json" } },
            new FilePickerFileType("All files") { Patterns = new[] { "*.*" } },
        },
        };

        var window = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window?.StorageProvider == null)
        {
            this.logger?.LogWarning("StorageProvider is null; unable to open file picker dialog.");
            return null;
        }

        var result = await window.StorageProvider.OpenFilePickerAsync(options);

        if (result == null || result.Count == 0)
        {
            this.logger?.LogInformation("No file selected in file picker dialog.");
            return null;
        }

        return result[0].TryGetLocalPath();
    }

    /// <summary>
    /// Initiates the migration task asynchronously.
    /// </summary>
    private Task RunMigrationTask()
    {
        return this.ExecuteMigrationAsync(token => this.migrationService.StartMigrationTaskAsync(token));
    }

    /// <summary>
    /// Resumes a migration process using the provided manifest file path.
    /// </summary>
    private Task ResumeMigrationTask(string manifestLoadFilePath)
    {
        return this.ExecuteMigrationAsync(token => this.migrationService.ResumeMigrationTaskAsync(manifestLoadFilePath, token));
    }

    /// <summary>
    /// Executes the migration task with the provided function, handling the migration result and notifications.
    /// </summary>
    private async Task ExecuteMigrationAsync(Func<CancellationToken, Task<DetailedMigrationResult>> migrationTaskFunc)
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
            DetailedMigrationResult migrationResult = await migrationTaskFunc(this.cancellationTokenSource.Token);

            switch (migrationResult.status)
            {
                case ITableauMigrationService.MigrationStatus.SUCCESS:
                    this.SetNotification("Migration Completed.", color: Brushes.Green);
                    break;

                case ITableauMigrationService.MigrationStatus.CANCELLED:
                    var details = this.BuildErrorDetails(migrationResult.errors);
                    this.SetNotification("Migration Cancelled.", details, Brushes.Red);
                    break;

                default:
                    var failureDetails = this.BuildErrorDetails(migrationResult.errors);
                    this.SetNotification("Migration Failed.", failureDetails, Brushes.Red);
                    break;
            }
        }
        else
        {
            this.SetNotification("Migration plan building failed.", color: Brushes.Red);
        }

        this.MessageDisplayVM.AddMessage(MigrationMessagesSessionSeperator);
        this.IsMigrating = false;
        this.progressUpdater.Reset();
    }

    /// <summary>
    /// Sets the notification message, details, and color for the UI.
    /// </summary>
    private void SetNotification(string message, string? detailsMessage = null, IImmutableSolidColorBrush? color = null)
    {
        this.NotificationMessage = message;
        this.NotificationDetailsMessage = detailsMessage ?? string.Empty;
        this.NotificationColor = color ?? Brushes.Black;
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

    /// <summary>
    /// Validates all fields and returns true if no errors are found; logs and throws if error count is invalid.
    /// </summary>
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