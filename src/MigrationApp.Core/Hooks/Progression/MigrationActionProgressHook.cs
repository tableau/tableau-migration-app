using Microsoft.Extensions.Logging;
using MigrationApp.Core.Interfaces;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;

namespace MigrationApp.Core.Hooks.Progression
{
    #region class
    /// <summary>
    /// Hook called at the end of each Migration Action from the SDK to update the Progress Bar on the GUI
    /// </summary>
    public class MigrationActionProgressHook : IMigrationActionCompletedHook
    {
        private readonly IProgressUpdater? _progressUpdater;
        public MigrationActionProgressHook(ILogger<MigrationActionProgressHook> logger, IProgressUpdater? progressUpdater)
        {
            _progressUpdater = progressUpdater;
        }

        public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
        {
            _progressUpdater?.Update();
            return Task.FromResult<IMigrationActionResult?>(ctx);
        }
    }
    #endregion
}