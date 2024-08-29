using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System;
using System.Collections;

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
        private string _serverToCloudUserDomainMap = string.Empty;

        private readonly Dictionary<string, List<string>> _errors = new();

        public string ServerUri
        {
            get => _serverUri;
            set
            {
                SetProperty(ref _serverUri, value);
                ValidateRequiredField(value, nameof(ServerUri), "Tableau Server URI is required.");
            }
        }

        public string ServerSiteContent
        {
            get => _serverSiteContent;
            set
            {
                SetProperty(ref _serverSiteContent, value);
                ValidateRequiredField(value, nameof(ServerSiteContent), "Tableau Server Site Content is required.");
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
            }
        }

        public string CloudSiteContent
        {
            get => _cloudSiteContent;
            set
            {
                SetProperty(ref _cloudSiteContent, value);
                ValidateRequiredField(value, nameof(CloudSiteContent), "Tableau Cloud Site Content is required.");
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

        public string ServerToCloudUserDomainMap
        {
            get => _serverToCloudUserDomainMap;
            set
            {
                SetProperty(ref _serverToCloudUserDomainMap, value);
                ValidateRequiredField(value, nameof(ServerToCloudUserDomainMap), "Tableau Server to Cloud User Domain Map is required.");
            }
        }

        public ICommand RunMigrationCommand { get; }

        public MainWindowViewModel()
        {
            RunMigrationCommand = new RelayCommand(RunMigration, CanExecuteRunMigration);
        }

        private void RunMigration()
        {
            ValidateRequiredField(ServerUri, nameof(ServerUri), "Tableau Server URI is required.");
            ValidateRequiredField(ServerSiteContent, nameof(ServerSiteContent), "Tableau Server Site Content is required.");
            ValidateRequiredField(ServerAccessTokenName, nameof(ServerAccessTokenName), "Tableau Server Access Token Name is required.");
            ValidateRequiredField(ServerAccessToken, nameof(ServerAccessToken), "Tableau Server Access Token is required.");
            ValidateRequiredField(CloudUri, nameof(CloudUri), "Tableau Cloud URI is required.");
            ValidateRequiredField(CloudSiteContent, nameof(CloudSiteContent), "Tableau Cloud Site Content is required.");
            ValidateRequiredField(CloudAccessTokenName, nameof(CloudAccessTokenName), "Tableau Cloud Access Token Name is required.");
            ValidateRequiredField(CloudAccessToken, nameof(CloudAccessToken), "Tableau Cloud Access Token is required.");
            ValidateRequiredField(ServerToCloudUserDomainMap, nameof(ServerToCloudUserDomainMap), "Tableau Server to Cloud User Domain Map is required.");

            if (HasErrors)
            {
                return;
            }

            // Proceed with the migration process
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
