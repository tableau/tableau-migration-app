using MigrationApp.Core.Interfaces;
using System;

namespace MigrationApp.GUI.Models
{
    public class ProgressUpdater : IProgressUpdater
    {
        private static readonly string[] _migrationStates = { "Users", "Groups", "Projects", "DataSources", "Workbooks", "ServerExtractRefreshTasks", "CustomViews" };
        private int _currentMigrationStateIndex = -1;
        public event EventHandler? OnProgressChanged;
        public int CurrentMigrationStateIndex
        {
            get => _currentMigrationStateIndex;
            set
            {
                if (_currentMigrationStateIndex != value && value <= _migrationStates.Length)
                {
                    _currentMigrationStateIndex = value;
                    OnProgressChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }


        public string CurrentMigrationStateName
        {
            get => CurrentMigrationStateIndex < 0 || CurrentMigrationStateIndex >= _migrationStates.Length ?
                string.Empty : _migrationStates[CurrentMigrationStateIndex];
        }

        public string CurrentMigrationMessage
        {
            get
            {
                if (CurrentMigrationStateIndex < 0)
                {
                    return String.Empty;
                }
                else if (CurrentMigrationStateIndex >= NumMigrationStates)
                {
                    return "Migration Finished.";
                }

                return $"Migrating {CurrentMigrationStateName}";

            }
        }

        public static int NumMigrationStates { get; } = _migrationStates.Length;

        /// <summary>
        /// Increases the current migration state to the next action.
        /// </summary>
        /// <remarks>
        /// This function exists for increased legibility when ProgressUpdater is nullable.
        /// </remarks>
        public void Update()
        {
            CurrentMigrationStateIndex++;
        }

        /// <summary>
        /// Resets the migration progress back to initial state.
        /// </summary>
        public void Reset()
        {
            CurrentMigrationStateIndex = -1;
        }
    }
}
