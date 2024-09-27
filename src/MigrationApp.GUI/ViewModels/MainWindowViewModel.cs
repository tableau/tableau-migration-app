// <copyright file="MainWindowViewModel.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;

using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Interfaces;
using MigrationApp.GUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// ViewModel containing the logic bindings for the main application window.
/// </summary>
public class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> errors = new ();
    private readonly ITableauMigrationService migrationService;
    private readonly IOptions<EmailDomainMappingOptions> emailDomainOptions;
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
    private bool isDomainMappingDisabled = false;
    private bool isMigrating = false;
    private IProgressUpdater progressUpdater;
    private string notificationMessage = string.Empty;
    private IImmutableSolidColorBrush notificationColor = Brushes.Black;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    /// <param name="migrationService">The migration service.</param>
    /// <param name="emailDomainOptions">The default domain mapping to apply to users who do not have an existing email, or mapping present.</param>
    /// <param name="progressUpdater">The object to track the migration progress from the migration service.</param>
    public MainWindowViewModel(
        ITableauMigrationService migrationService,
        IOptions<EmailDomainMappingOptions> emailDomainOptions,
        IProgressUpdater progressUpdater)
    {
        this.migrationService = migrationService;
        this.emailDomainOptions = emailDomainOptions;
        this.progressUpdater = progressUpdater;
        this.RunMigrationCommand = new RelayCommand(this.RunMigration, this.CanExecuteRunMigration);

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
    /// Gets the command to run the migration.
    /// </summary>
    public ICommand RunMigrationCommand { get; }

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
                this.isMigrating = value;
                this.OnPropertyChanged(nameof(this.IsMigrating)); // Notify UI about the change
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
    /// Gets or sets a value indicating whether gets or Sets whether domain mapping enabled or disabled.
    /// </summary>
    public bool IsDomainMappingDisabled
    {
        get => this.isDomainMappingDisabled;
        set
        {
            this.isDomainMappingDisabled = value;
            this.OnPropertyChanged(nameof(this.IsDomainMappingDisabled));
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
    /// Gets a value indicating whether gets whether any validation errors detected.
    /// </summary>
    public bool HasErrors => this.errors.Any();

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
            bool success = await this.migrationService.StartMigrationTaskAsync(CancellationToken.None);

            if (success)
            {
                this.NotificationMessage = "Migration succeeded.";
                this.NotificationColor = Brushes.Green;
            }
            else
            {
                this.NotificationMessage = "Migration failed.";
                this.NotificationColor = Brushes.Red;
            }
        }
        else
        {
            this.NotificationMessage = "Migration plan building failed.";
            this.NotificationColor = Brushes.Red;
        }

        this.IsMigrating = false;
        this.progressUpdater.Reset();
    }

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
        this.RunMigrationAsync().ConfigureAwait(false);
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
        var domainPattern = @"^((?!-)[A-Za-z0-9-]{1,63}(?<!-)\.)+[A-Za-z]{2,20}$";

        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, domainPattern))
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

/// <summary>
/// Defines a command with execution and can-execute logic.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action execute;

    private readonly Func<bool>? canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand" /> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">Whether or not the execute action can be called.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data, this object can be set to null.</param>
    /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
    public bool CanExecute(object? parameter) => this.canExecute?.Invoke() ?? true;

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data, this object can be set to null.</param>
    public void Execute(object? parameter) => this.execute();

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}