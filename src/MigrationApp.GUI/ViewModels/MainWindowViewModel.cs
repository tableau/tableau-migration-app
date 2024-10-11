// <copyright file="MainWindowViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;

using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Interfaces;
using MigrationApp.GUI.Models;
using MigrationApp.GUI.Services.Implementations;
using MigrationApp.GUI.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// ViewModel containing the logic bindings for the main application window.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private const string MigrationMessagesSessionSeperator = "--------------------------------------------";
    private readonly Dictionary<string, List<string>> errors = new ();
    private readonly ITableauMigrationService migrationService;
    private readonly IOptions<EmailDomainMappingOptions> emailDomainOptions;
    private readonly IOptions<DictionaryUserMappingOptions> dictionaryUserMappingOptions;
    private string serverSiteContent = string.Empty;
    private string serverAccessTokenName = string.Empty;
    private string serverAccessToken = string.Empty;
    private string cloudSiteContent = string.Empty;
    private string cloudAccessTokenName = string.Empty;
    private string cloudAccessToken = string.Empty;
    private string cloudUserDomain = string.Empty;
    private string serverUriFull = string.Empty;
    private string serverUriBase = string.Empty;
    private string cloudUriFull = string.Empty;
    private string cloudUriBase = string.Empty;
    private string loadedCsvFilename = "No file loaded.";
    private string csvLoadStatus = string.Empty;
    private bool isDomainMappingDisabled = false;
    private bool isMigrating = false;
    private bool isUserMappingFileLoaded = false;
    private IProgressUpdater progressUpdater;
    private IFilePicker filePicker;
    private ICsvParser csvParser;
    private string notificationMessage = string.Empty;
    private string notificationDetailsMessage = string.Empty;
    private CancellationTokenSource? cancellationTokenSource = null;
    private IImmutableSolidColorBrush notificationColor = Brushes.Black;
    private IImmutableSolidColorBrush csvLoadStatusColor = Brushes.Black;
    private Dictionary<string, string> userMappings = new Dictionary<string, string>();
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
        this.MessageDisplayVM = new MessageDisplayViewModel(publisher);
        this.emailDomainOptions = emailDomainOptions;
        this.dictionaryUserMappingOptions = dictionaryUserMappingOptions;
        this.progressUpdater = progressUpdater;
        this.filePicker = filePicker;
        this.csvParser = csvParser;
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
                this.OnPropertyChanged(nameof(this.IsDomainMappingVisiblyDisabled)); // Notify UI about the change
            }
        }
    }

    /// <summary>
    /// Gets or Sets the URI to be used for the source Tableau Server.
    /// </summary>
    public string ServerUriFull
    {
        get => this.serverUriFull;
        set
        {
            this.SetProperty(ref this.serverUriFull, value);

            this.ValidateRequiredField(value, nameof(this.ServerUriFull), "Tableau Server URI is required.");
            this.ValidateUriFormat(value, nameof(this.ServerUriFull), "Invalid URI format for Tableau Server URI.");

            this.ServerUriBase = this.ExtractBaseUri(value);
            this.ServerSiteContent = this.ExtractSiteContent(value);
        }
    }

    /// <summary>
    /// Gets or sets the base URI of the source Tableau Server.
    /// </summary>
    public string ServerUriBase
    {
        get => this.serverUriBase;
        set
        {
            this.SetProperty(ref this.serverUriBase, value);
        }
    }

    /// <summary>
    /// Gets or Sets the site to be used for the source Tableau Server.
    /// </summary>
    public string ServerSiteContent
    {
        get => this.serverSiteContent;
        set
        {
            this.SetProperty(ref this.serverSiteContent, value);
        }
    }

    /// <summary>
    /// Gets or Sets the name of the Personal Access Token to be used for the source Tableau Server.
    /// </summary>
    public string ServerAccessTokenName
    {
        get => this.serverAccessTokenName;
        set
        {
            this.SetProperty(ref this.serverAccessTokenName, value);
            this.ValidateRequiredField(value, nameof(this.ServerAccessTokenName), "Tableau Server Access Token Name is required.");
        }
    }

    /// <summary>
    /// Gets or Sets the Personal Access Token to be used for the source Tableau Server.
    /// </summary>
    public string ServerAccessToken
    {
        get => this.serverAccessToken;
        set
        {
            this.SetProperty(ref this.serverAccessToken, value);
            this.ValidateRequiredField(value, nameof(this.ServerAccessToken), "Tableau Server Access Token is required.");
        }
    }

    /// <summary>
    /// Gets or Sets the full URI to be used for the destination Tableau Cloud.
    /// </summary>
    public string CloudUriFull
    {
        get => this.cloudUriFull;
        set
        {
            this.SetProperty(ref this.cloudUriFull, value);

            this.ValidateRequiredField(value, nameof(this.CloudUriFull), "Tableau Cloud URI is required.");
            this.ValidateUriFormat(value, nameof(this.CloudUriFull), "Invalid URI format for Tableau Cloud URI.");

            this.CloudUriBase = this.ExtractBaseUri(value);
            this.CloudSiteContent = this.ExtractSiteContent(value);
        }
    }

    /// <summary>
    /// Gets or sets the Tableau Cloud base URI.
    /// </summary>
    public string CloudUriBase
    {
        get => this.cloudUriBase;
        set
        {
            this.SetProperty(ref this.cloudUriBase, value);
        }
    }

    /// <summary>
    /// Gets or Sets the Site to be used for the destination Tableau Cloud.
    /// </summary>
    public string CloudSiteContent
    {
        get => this.cloudSiteContent;
        set
        {
            this.SetProperty(ref this.cloudSiteContent, value);
        }
    }

    /// <summary>
    /// Gets or Sets the name of the Personal Access Token to be used for the destination Tableau Cloud.
    /// </summary>
    public string CloudAccessTokenName
    {
        get => this.cloudAccessTokenName;
        set
        {
            this.SetProperty(ref this.cloudAccessTokenName, value);
            this.ValidateRequiredField(value, nameof(this.CloudAccessTokenName), "Tableau Cloud Access Token Name is required.");
        }
    }

    /// <summary>
    /// Gets or Sets the Personal Access Token to be used for the destination Tableau Cloud.
    /// </summary>
    public string CloudAccessToken
    {
        get => this.cloudAccessToken;
        set
        {
            this.SetProperty(ref this.cloudAccessToken, value);
            this.ValidateRequiredField(value, nameof(this.CloudAccessToken), "Tableau Cloud Access Token is required.");
        }
    }

    /// <summary>
    /// Gets or Sets the default domain to be mapped to users that do not have an existing mapping, or an assigned email on the source Tableau Server.
    /// </summary>
    public string CloudUserDomain
    {
        get => this.cloudUserDomain;
        set
        {
            this.SetProperty(ref this.cloudUserDomain, value);
            this.ValidateRequiredField(value, nameof(this.CloudUserDomain), "Tableau Server to Cloud User Domain Map is required.");
            this.ValidateDomainName(value, nameof(this.CloudUserDomain), "The provided value is not a valid domain.");
            this.emailDomainOptions.Value.EmailDomain = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the Domain Mapping should be visibly disabled.
    /// </summary>
    public bool IsDomainMappingVisiblyDisabled => this.IsDomainMappingDisabled || this.IsMigrating;

    /// <summary>
    /// Gets or sets a value indicating whether gets or Sets whether domain mapping enabled or disabled.
    /// </summary>
    public bool IsDomainMappingDisabled
    {
        get => this.isDomainMappingDisabled;
        set
        {
            this.isDomainMappingDisabled = value;
            this.OnPropertyChanged(nameof(this.IsDomainMappingDisabled));
            this.OnPropertyChanged(nameof(this.IsDomainMappingVisiblyDisabled));
            if (this.isDomainMappingDisabled)
            {
                this.RemoveError(nameof(this.CloudUserDomain), "Tableau Server to Cloud User Domain Map is required.");
                this.RemoveError(nameof(this.CloudUserDomain), "The provided value is not a valid domain.");
                this.emailDomainOptions.Value.EmailDomain = string.Empty;
            }
            else
            {
                this.ValidateRequiredField(this.CloudUserDomain, nameof(this.CloudUserDomain), "Tableau Server to Cloud User Domain Map is required.");
                this.ValidateDomainName(this.CloudUserDomain, nameof(this.CloudUserDomain), "The provided value is not a valid domain.");
                this.emailDomainOptions.Value.EmailDomain = this.CloudUserDomain;
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
    /// Gets a value indicating whether any validation errors are detected.
    /// </summary>
    public bool HasErrors => this.errors.Any();

    /// <summary>
    /// Gets or sets the loaded CSV filename used for username mappings.
    /// </summary>
    public string LoadedCSVFilename
    {
        get => this.loadedCsvFilename;
        set
        {
            if (value == string.Empty)
            {
                this.SetProperty(ref this.loadedCsvFilename, "No file loaded.");
                return;
            }

            this.SetProperty(ref this.loadedCsvFilename, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a CSV used for user mapping is loaded.
    /// </summary>
    public bool IsUserMappingFileLoaded
    {
        get => this.isUserMappingFileLoaded;
        set => this.SetProperty(ref this.isUserMappingFileLoaded, value);
    }

    /// <summary>
    /// Gets or sets the status message displayed after loading the user mapping file.
    /// </summary>
    public string CSVLoadStatus
    {
        get => this.csvLoadStatus;
        set => this.SetProperty(ref this.csvLoadStatus, value);
    }

    /// <summary>
    /// Gets or Sets the color to be used for the CSV load status  message.
    /// </summary>
    public IImmutableSolidColorBrush CSVLoadStatusColor
    {
        get => this.csvLoadStatusColor;
        set => this.SetProperty(ref this.csvLoadStatusColor, value);
    }

    /// <summary>
    /// Gets the errors for a given property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The error associated for this property, if exists.</returns>
    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == null)
        {
            return this.errors.Values.SelectMany(e => e).ToList();
        }

        return this.errors.ContainsKey(propertyName) ? this.errors[propertyName] : Enumerable.Empty<string>();
    }

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
            Url = new Uri(this.ServerUriBase),
            SiteContentUrl = this.ServerSiteContent,
            AccessTokenName = this.ServerAccessTokenName,
            AccessToken = this.ServerAccessToken,
        };

        var cloudCreds = new EndpointOptions
        {
            Url = new Uri(this.CloudUriBase),
            SiteContentUrl = this.CloudSiteContent,
            AccessTokenName = this.CloudAccessTokenName,
            AccessToken = this.CloudAccessToken,
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
        this.ValidateRequiredField(this.ServerUriFull, nameof(this.ServerUriFull), "Tableau Server URI is required.");
        this.ValidateUriFormat(this.ServerUriFull, nameof(this.ServerUriFull), "Invalid URI format for Tableau Server URI.");
        this.ValidateRequiredField(this.CloudUriFull, nameof(this.CloudUriFull), "Tableau Cloud URI is required.");
        this.ValidateUriFormat(this.CloudUriFull, nameof(this.CloudUriFull), "Invalid URI format for Tableau Cloud URI.");
        this.ValidateRequiredField(this.ServerAccessTokenName, nameof(this.ServerAccessTokenName), "Tableau Server Access Token Name is required.");
        this.ValidateRequiredField(this.ServerAccessToken, nameof(this.ServerAccessToken), "Tableau Server Access Token is required.");
        this.ValidateRequiredField(this.CloudAccessTokenName, nameof(this.CloudAccessTokenName), "Tableau Cloud Access Token Name is required.");
        this.ValidateRequiredField(this.CloudAccessToken, nameof(this.CloudAccessToken), "Tableau Cloud Access Token is required.");
        if (!this.isDomainMappingDisabled)
        {
            this.ValidateRequiredField(this.CloudUserDomain, nameof(this.CloudUserDomain), "Tableau Server to Cloud User Domain Map is required.");
        }

        if (this.HasErrors)
        {
            return;
        }

        this.IsMigrating = true;
        this.MessageDisplayVM.AddMessage(MigrationMessagesSessionSeperator);
        this.MessageDisplayVM.AddMessage("Migration Started");
        this.logger?.LogInformation("Migration Started");
        this.RunMigrationAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private void UnLoadUserFile()
    {
        this.ClearCSVLoadedValues();
        this.CSVLoadStatus = string.Empty;
        this.CSVLoadStatusColor = Brushes.Black;
    }

    [RelayCommand]
    private async Task LoadUserFile()
    {
        try
        {
            var file = await this.filePicker.OpenFilePickerAsync("Load User Mappings");

            if (file is null)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = "Could not find file.";
                return;
            }

            string? localPath = file.TryGetLocalPath();
            if (localPath == null)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = "Could not find file.";
                return;
            }

            try
            {
                this.userMappings = await this.csvParser.ParseAsync(localPath);
                this.dictionaryUserMappingOptions.Value.UserMappings = this.userMappings;
                this.LoadedCSVFilename = file.Name;
                this.IsUserMappingFileLoaded = true;
                this.CSVLoadStatusColor = Brushes.Black;
                this.CSVLoadStatus = $"{this.userMappings.Count} user mappings loaded.";
            }
            catch (InvalidDataException e)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = $"Failed to load {file.Name}.\n{e.Message}";
                this.ClearCSVLoadedValues();
            }
            catch (FileNotFoundException)
            {
                this.CSVLoadStatusColor = Brushes.Red;
                this.CSVLoadStatus = "Could not find file.";
                this.CSVLoadStatusColor = Brushes.Red;
                this.ClearCSVLoadedValues();
            }
        }
        catch (Exception e)
        {
            this.CSVLoadStatusColor = Brushes.Red;
            this.CSVLoadStatus = $"Encountered an unexpected error with file picker.\n{e.Message}";
            this.CSVLoadStatusColor = Brushes.Red;
            this.ClearCSVLoadedValues();
        }

        return;
    }

    private void ClearCSVLoadedValues()
    {
        this.IsUserMappingFileLoaded = false;
        this.LoadedCSVFilename = string.Empty;
        this.userMappings.Clear();
        this.dictionaryUserMappingOptions.Value.UserMappings = this.userMappings;
    }

    private string ExtractBaseUri(string uri)
    {
        if (Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
        {
            return $"{parsedUri.Scheme}://{parsedUri.Host}/";
        }

        return string.Empty;
    }

    private string ExtractSiteContent(string uri)
    {
        if (Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
        {
            var fragment = parsedUri.Fragment;

            var fragmentSegments = fragment.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < fragmentSegments.Length; i++)
            {
                if (fragmentSegments[i].Equals("site", StringComparison.OrdinalIgnoreCase) && i + 1 < fragmentSegments.Length)
                {
                    return fragmentSegments[i + 1];
                }
            }
        }

        return string.Empty;
    }

    private bool CanExecuteRunMigration() => !this.HasErrors;

    private void ValidateRequiredField(string value, string propertyName, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            this.AddError(propertyName, errorMessage);
        }
        else
        {
            this.RemoveError(propertyName, errorMessage);
        }
    }

    private void ValidateUriFormat(string value, string propertyName, string errorMessage)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            this.AddError(propertyName, errorMessage);
        }
        else
        {
            this.RemoveError(propertyName, errorMessage);
        }
    }

    private void ValidateDomainName(string value, string propertyName, string errorMessage)
    {
        if (!Validator.IsDomainNameValid(value))
        {
            this.AddError(propertyName, errorMessage);
        }
        else
        {
            this.RemoveError(propertyName, errorMessage);
        }
    }

    private void AddError(string propertyName, string errorMessage)
    {
        if (!this.errors.ContainsKey(propertyName))
        {
            this.errors[propertyName] = new List<string>();
        }

        if (!this.errors[propertyName].Contains(errorMessage))
        {
            this.errors[propertyName].Add(errorMessage);
            this.OnErrorsChanged(propertyName);
        }
    }

    private void RemoveError(string propertyName, string errorMessage)
    {
        if (this.errors.ContainsKey(propertyName) && this.errors[propertyName].Contains(errorMessage))
        {
            this.errors[propertyName].Remove(errorMessage);
            if (!this.errors[propertyName].Any())
            {
                this.errors.Remove(propertyName);
            }

            this.OnErrorsChanged(propertyName);
        }
    }
}