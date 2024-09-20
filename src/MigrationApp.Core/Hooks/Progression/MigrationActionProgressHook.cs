// <copyright file="MigrationActionProgressHook.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Hooks.Progression
{
    using Microsoft.Extensions.Logging;
    using MigrationApp.Core.Interfaces;
    using Tableau.Migration.Engine.Actions;
    using Tableau.Migration.Engine.Hooks;

    /// <summary>
    /// Hook called at the end of each Migration Action from the SDK to update the Progress Bar on the GUI.
    /// </summary>
    public class MigrationActionProgressHook : IMigrationActionCompletedHook
    {
        private readonly IProgressUpdater? progressUpdater;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationActionProgressHook"/> class.
        /// Action hook set to trigger migration progress visualizations.
        /// </summary>
        /// <param name="progressUpdater">The object to track the migration progress.</param>
        public MigrationActionProgressHook(IProgressUpdater? progressUpdater)
        {
            this.progressUpdater = progressUpdater;
        }

        /// <inheritdoc/>
        public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
        {
            this.progressUpdater?.Update();
            return Task.FromResult<IMigrationActionResult?>(ctx);
        }
    }
}