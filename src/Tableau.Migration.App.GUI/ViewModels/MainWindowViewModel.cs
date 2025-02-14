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

using Avalonia.Controls;
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
using System.Text;
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
    private const string MigrationMessagesSessionSeperator = "─────────────────────────────";
    private readonly ITableauMigrationService migrationService;
    private bool isMigrating = false;
    private IProgressUpdater progressUpdater;
    private IProgressMessagePublisher publisher;
    private IMigrationTimer migrationTimer;
    private string notificationMessage = string.Empty;
    private CancellationTokenSource? cancellationTokenSource = null;
    private IImmutableSolidColorBrush notificationColor = Brushes.Black;
    private ILogger<MainWindowViewModel>? logger;
    private TimersViewModel timersVM;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    /// <param name="migrationService">The migration service.</param>
    /// <param name="progressUpdater">The object to track the migration progress from the migration service.</param>
    /// <param name="migrationTimer">The object to track the migration times.</param>
    /// <param name="publisher">The progress publisher for progress status messages.</param>
    /// <param name="timersVM">The Timers view model.</param>
    /// <param name="userMappingsVM">The User Mappings view model.</param>
    public MainWindowViewModel(
        ITableauMigrationService migrationService,
        IProgressUpdater progressUpdater,
        IProgressMessagePublisher publisher,
        IMigrationTimer migrationTimer,
        TimersViewModel timersVM,
        UserMappingsViewModel userMappingsVM)
    {
        this.migrationService = migrationService;
        this.publisher = publisher;
        this.timersVM = timersVM;
        this.UserMappingsVM = userMappingsVM;

        // Sub View Models for Authentication Credentials
        this.ServerCredentialsVM = new AuthCredentialsViewModel(TableauEnv.TableauServer);
        this.CloudCredentialsVM = new AuthCredentialsViewModel(TableauEnv.TableauCloud);

        this.progressUpdater = progressUpdater;
        this.migrationTimer = migrationTimer;
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

            this.OnPropertyChanged(nameof(this.IsNotificationVisible));
        }
    }

    /// <summary>
    /// Gets or Sets the notification message under the run migration button.
    /// </summary>
    public string NotificationMessage
    {
        get => this.notificationMessage;
        set
        {
            this.SetProperty(ref this.notificationMessage, value);
            this.OnPropertyChanged(nameof(this.IsNotificationVisible));
        }
    }

    /// <summary>
    /// Gets a value indicating whether notification message should be visible.
    /// </summary>
    public bool IsNotificationVisible
    {
        get => !string.IsNullOrEmpty(this.NotificationMessage) && !this.IsMigrating;
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
        if (string.IsNullOrEmpty(manifestSaveFilePath))
        {
            this.NotificationMessage += " Manifest was not saved.";
            return;
        }

        bool isSaved = await this.migrationService.SaveManifestAsync(manifestSaveFilePath);
        this.NotificationMessage += isSaved ? " Manifest saved." : " Failed to save manifest.";
    }

    /// <summary>
    /// Validates fields and starts the migration process if valid.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the migration.</returns>
    [RelayCommand]
    public async Task RunMigration()
    {
        if (!this.AreFieldsValid())
        {
            this.logger?.LogInformation("Migration Run failed due to validation errors.");
            return;
        }

        this.IsMigrating = true;
        this.publisher.PublishProgressMessage("Migration Started");
        this.logger?.LogInformation("Migration Started");
        this.migrationTimer.Reset(); // Setup migration timer to store progress timing states
        this.migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationStarted);
        this.timersVM.Start(); // Triggers for the Timer View to start polling for information
        await this.RunMigrationTask().ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously resumes migration by selecting a manifest file and calling RunResumeMigration with the file path.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the async migration.</returns>
    [RelayCommand]
    public async Task ResumeMigration()
    {
        var filePath = await this.SelectManifestFileAsync();
        if (filePath != null)
        {
            await this.RunMigration();
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

        // Retrieve the active application window to access the file picker
        var window = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window?.StorageProvider == null)
        {
            this.logger?.LogWarning("StorageProvider is null; unable to open file picker dialog.");
            return null;
        }

        // Open file picker and return the selected file path
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
            // If we have a plan, then run the migration
            this.cancellationTokenSource = new CancellationTokenSource();
            DetailedMigrationResult migrationResult = await migrationTaskFunc(this.cancellationTokenSource.Token);
            this.PublishMigrationResult(migrationResult);
        }
        else
        {
            this.SetNotification("Migration plan building failed.", color: Brushes.Red);
        }

        string path = AppContext.BaseDirectory;
        bool isManifestSaved = await this.migrationService.SaveManifestAsync($"{path}/manifest.json");

        if (isManifestSaved)
        {
            this.publisher.PublishProgressMessage($"Manifest saved to '{path}manifest.json'");
        }

        this.publisher.PublishProgressMessage($"{Environment.NewLine}Total Elapsed Time: {this.migrationTimer.GetTotalMigrationTime}");
        this.publisher.PublishProgressMessage(MigrationMessagesSessionSeperator);

        this.IsMigrating = false;
        this.timersVM.Stop();
        this.progressUpdater.Reset();
    }

    /// <summary>
    /// Sets the notification message, details, and color for the UI.
    /// </summary>
    private void SetNotification(string message, string? detailsMessage = null, IImmutableSolidColorBrush? color = null)
    {
        this.NotificationMessage = message;
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

    private void PublishMigrationResult(DetailedMigrationResult migrationResult)
    {
        switch (migrationResult.status)
        {
            case ITableauMigrationService.MigrationStatus.SUCCESS:
                this.SetNotification("Migration completed.", color: Brushes.Green);
                this.publisher.PublishProgressMessage("Migration completed.");
                break;

            case ITableauMigrationService.MigrationStatus.CANCELED:
                var details = this.BuildErrorDetails(migrationResult.errors);
                this.SetNotification("Migration canceled.", details, Brushes.Red);
                this.publisher.PublishProgressMessage("Migration canceled.");
                break;

            default:
                var failureDetails = this.BuildErrorDetails(migrationResult.errors);
                this.SetNotification("Migration Failed.", failureDetails, Brushes.Red);
                this.publisher.PublishProgressMessage("Migration failed.");
                break;
        }

        this.PublishMigrationErrors(migrationResult.errors);
    }

    private void PublishMigrationErrors(IReadOnlyList<Exception> errors)
    {
        StringBuilder sb = new StringBuilder();
        var statusIcon =
            IProgressMessagePublisher
            .GetStatusIcon(IProgressMessagePublisher.MessageStatus.Error);
        foreach (var error in errors)
        {
            try
            {
                ErrorMessage parsedError = new ErrorMessage(error.Message);
                sb.Append($"\t {statusIcon} {parsedError.Detail}");
                sb.Append($"\t\t {parsedError.Summary}: {parsedError.URL}");
            }
            catch (Exception)
            {
                sb.Append(
                    $"\t {statusIcon} Could not parse error message:{Environment.NewLine}{error.Message}");
            }
        }

        string statusMessages = sb.ToString();
        this.publisher.PublishProgressMessage(statusMessages);
    }
}