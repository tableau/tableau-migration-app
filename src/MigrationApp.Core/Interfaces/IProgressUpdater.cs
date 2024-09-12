namespace MigrationApp.Core.Interfaces;

public interface IProgressUpdater
{
    int CurrentMigrationStateIndex { get; set; }
    string CurrentMigrationStateName { get; }

    string CurrentMigrationMessage { get; }
    static int NumMigrationStates { get; }

    event EventHandler? OnProgressChanged;
    void Update();

    void Reset();
}
