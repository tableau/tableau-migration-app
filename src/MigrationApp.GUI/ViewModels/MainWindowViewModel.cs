using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System;
using System.Collections;
using MigrationApp.Core.Interfaces;
using MigrationApp.Core.Entities;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Media;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;

namespace MigrationApp.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private string _serverUri = string.Empty;
        private string _serverSiteContent = string.Empty;
        private string _serverAccessTokenName = string.Empty;
        private string _serverAccessToken = string.Empty;
        private string _cloudUri = string.Empty;
        private string _cloudSiteContent = string.Empty;
        private string _cloudAccessTokenName = string.Empty;
        private string _cloudAccessToken = string.Empty;
        private string _cloudUserDomain = string.Empty;
        private bool _isMigrating = false;

        private readonly Dictionary<string, List<string>> _errors = new();

        private readonly ITableauMigrationService _migrationService;

        private readonly IOptions<EmailDomainMappingOptions> _emailDomainOptions;

        public ICommand RunMigrationCommand { get; }
        public bool IsMigrating { get => _isMigrating;
            set
            {
                if (_isMigrating != value)
                {
                    _isMigrating = value;
                    OnPropertyChanged(nameof(IsMigrating)); // Notify UI about the change
                }
            }
        }

        public MainWindowViewModel(ITableauMigrationService migrationService, IOptions<EmailDomainMappingOptions> emailDomainOptions)
        {
            _migrationService = migrationService;
            RunMigrationCommand = new RelayCommand(RunMigration, CanExecuteRunMigration);
            _emailDomainOptions = emailDomainOptions;
        }

        private async Task RunMigrationAsync()
        {
            var serverCreds = new EndpointOptions
            {
                Url = new Uri(ServerUri),
                SiteContentUrl = ServerSiteContent,
                AccessTokenName = ServerAccessTokenName,
                AccessToken = ServerAccessToken
            };

            var cloudCreds = new EndpointOptions
            {
                Url = new Uri(CloudUri),
                SiteContentUrl = CloudSiteContent,
                AccessTokenName = CloudAccessTokenName,
                AccessToken = CloudAccessToken
            };

            bool planBuilt = _migrationService.BuildMigrationPlan(serverCreds, cloudCreds);

            if (planBuilt)
            {
                bool success = await _migrationService.StartMigrationTaskAsync(CancellationToken.None);

                if (success)
                {
                    NotificationMessage = "Migration succeeded.";
                    NotificationColor = Brushes.Green;
                }
                else
                {
                    NotificationMessage = "Migration failed.";
                    NotificationColor = Brushes.Red;
                }
            }
            else
            {
                NotificationMessage = "Migration plan building failed.";
                NotificationColor = Brushes.Red;
            }
            IsMigrating = false;
        }

        #region Properties

        public string ServerUri
        {
            get => _serverUri;
            set
            {
                SetProperty(ref _serverUri, value);
                ValidateRequiredField(value, nameof(ServerUri), "Tableau Server URI is required.");
                ValidateUriFormat(value, nameof(ServerUri), "Invalid URI format for Tableau Server URI.");
            }
        }

        public string ServerSiteContent
        {
            get => _serverSiteContent;
            set
            {
                SetProperty(ref _serverSiteContent, value);
            }
        }

        public string ServerAccessTokenName
        {
            get => _serverAccessTokenName;
            set
            {
                SetProperty(ref _serverAccessTokenName, value);
                ValidateRequiredField(value, nameof(ServerAccessTokenName), "Tableau Server Access Token Name is required.");
            }
        }

        public string ServerAccessToken
        {
            get => _serverAccessToken;
            set
            {
                SetProperty(ref _serverAccessToken, value);
                ValidateRequiredField(value, nameof(ServerAccessToken), "Tableau Server Access Token is required.");
            }
        }

        public string CloudUri
        {
            get => _cloudUri;
            set
            {
                SetProperty(ref _cloudUri, value);
                ValidateRequiredField(value, nameof(CloudUri), "Tableau Cloud URI is required.");
                ValidateUriFormat(value, nameof(CloudUri), "Invalid URI format for Tableau Cloud URI.");
            }
        }

        public string CloudSiteContent
        {
            get => _cloudSiteContent;
            set
            {
                SetProperty(ref _cloudSiteContent, value);
            }
        }

        public string CloudAccessTokenName
        {
            get => _cloudAccessTokenName;
            set
            {
                SetProperty(ref _cloudAccessTokenName, value);
                ValidateRequiredField(value, nameof(CloudAccessTokenName), "Tableau Cloud Access Token Name is required.");
            }
        }

        public string CloudAccessToken
        {
            get => _cloudAccessToken;
            set
            {
                SetProperty(ref _cloudAccessToken, value);
                ValidateRequiredField(value, nameof(CloudAccessToken), "Tableau Cloud Access Token is required.");
            }
        }

        public string CloudUserDomain
        {
            get => _cloudUserDomain;
            set
            {
                SetProperty(ref _cloudUserDomain, value);
                ValidateRequiredField(value, nameof(CloudUserDomain), "Tableau Server to Cloud User Domain Map is required.");
                _emailDomainOptions.Value.EmailDomain = value;
            }
        }

        private string _notificationMessage = string.Empty;
        private IImmutableSolidColorBrush _notificationColor = Brushes.Black;

        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetProperty(ref _notificationMessage, value);
        }

        public IImmutableSolidColorBrush NotificationColor
        {
            get => _notificationColor;
            set => SetProperty(ref _notificationColor, value);
        }
        #endregion


        private void RunMigration()
        {
            ValidateRequiredField(ServerUri, nameof(ServerUri), "Tableau Server URI is required.");
            ValidateUriFormat(ServerUri, nameof(ServerUri), "Invalid URI format for Tableau Server URI.");
            ValidateRequiredField(CloudUri, nameof(CloudUri), "Tableau Cloud URI is required.");
            ValidateUriFormat(CloudUri, nameof(CloudUri), "Invalid URI format for Tableau Cloud URI.");
            ValidateRequiredField(ServerAccessTokenName, nameof(ServerAccessTokenName), "Tableau Server Access Token Name is required.");
            ValidateRequiredField(ServerAccessToken, nameof(ServerAccessToken), "Tableau Server Access Token is required.");
            ValidateRequiredField(CloudAccessTokenName, nameof(CloudAccessTokenName), "Tableau Cloud Access Token Name is required.");
            ValidateRequiredField(CloudAccessToken, nameof(CloudAccessToken), "Tableau Cloud Access Token is required.");
            ValidateRequiredField(CloudUserDomain, nameof(CloudUserDomain), "Tableau Server to Cloud User Domain Map is required.");

            if (HasErrors)
            {
                return;
            }
            IsMigrating = true;
            RunMigrationAsync().ConfigureAwait(false);
        }

        private bool CanExecuteRunMigration() => !HasErrors;

        private void ValidateRequiredField(string value, string propertyName, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(propertyName, errorMessage);
            }
            else
            {
                RemoveError(propertyName, errorMessage);
            }
        }

        private void ValidateUriFormat(string value, string propertyName, string errorMessage)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                AddError(propertyName, errorMessage);
            }
            else
            {
                RemoveError(propertyName, errorMessage);
            }
        }

        #region INotifyDataErrorInfo Implementation

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errors.Any();

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName == null)
            {
                return _errors.Values.SelectMany(e => e).ToList();
            }

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                OnErrorsChanged(propertyName);
            }
        }

        private void RemoveError(string propertyName, string errorMessage)
        {
            if (_errors.ContainsKey(propertyName) && _errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Remove(errorMessage);
                if (!_errors[propertyName].Any())
                {
                    _errors.Remove(propertyName);
                }
                OnErrorsChanged(propertyName);
            }
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
