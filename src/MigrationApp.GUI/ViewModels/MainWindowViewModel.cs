using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System;
using System.Collections;

namespace EsmbMigration.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private string _serverUrl = string.Empty;
        private string _adminUserName = string.Empty;
        private string _personalAccessToken = string.Empty;
        private string _cloudUrl = string.Empty;
        private string _adminUserEmail = string.Empty;
        private string _cloudPersonalAccessToken = string.Empty;

        private readonly Dictionary<string, List<string>> _errors = new();

        public string ServerUrl
        {
            get => _serverUrl;
            set
            {
                SetProperty(ref _serverUrl, value);
                ValidateRequiredField(value, nameof(ServerUrl), "Server URL is required.");
            }
        }

        public string AdminUserName
        {
            get => _adminUserName;
            set
            {
                SetProperty(ref _adminUserName, value);
                ValidateRequiredField(value, nameof(AdminUserName), "Admin Username is required.");
            }
        }

        public string PersonalAccessToken
        {
            get => _personalAccessToken;
            set
            {
                SetProperty(ref _personalAccessToken, value);
                ValidateRequiredField(value, nameof(PersonalAccessToken), "Personal Access Token is required.");
            }
        }

        public string CloudUrl
        {
            get => _cloudUrl;
            set
            {
                SetProperty(ref _cloudUrl, value);
                ValidateRequiredField(value, nameof(CloudUrl), "Cloud URL is required.");
            }
        }

        public string AdminUserEmail
        {
            get => _adminUserEmail;
            set
            {
                SetProperty(ref _adminUserEmail, value);
                ValidateRequiredField(value, nameof(AdminUserEmail), "Admin User Email is required.");
                ValidateEmail(value, nameof(AdminUserEmail), "Invalid email address.");
            }
        }

        public string CloudPersonalAccessToken
        {
            get => _cloudPersonalAccessToken;
            set
            {
                SetProperty(ref _cloudPersonalAccessToken, value);
                ValidateRequiredField(value, nameof(CloudPersonalAccessToken), "Cloud Personal Access Token is required.");
            }
        }

        public ICommand RunMigrationCommand { get; }

        public MainWindowViewModel()
        {
            RunMigrationCommand = new RelayCommand(RunMigration, CanExecuteRunMigration);
        }

        private void RunMigration()
        {
            ValidateRequiredField(ServerUrl, nameof(ServerUrl), "Server URL is required.");
            ValidateRequiredField(AdminUserName, nameof(AdminUserName), "Admin Username is required.");
            ValidateRequiredField(PersonalAccessToken, nameof(PersonalAccessToken), "Personal Access Token is required.");
            ValidateRequiredField(CloudUrl, nameof(CloudUrl), "Cloud URL is required.");
            ValidateRequiredField(AdminUserEmail, nameof(AdminUserEmail), "Admin User Email is required.");
            ValidateEmail(AdminUserEmail, nameof(AdminUserEmail), "Invalid email address.");
            ValidateRequiredField(CloudPersonalAccessToken, nameof(CloudPersonalAccessToken), "Cloud Personal Access Token is required.");

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

        private void ValidateEmail(string value, string propertyName, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(value) && !value.Contains("@"))
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