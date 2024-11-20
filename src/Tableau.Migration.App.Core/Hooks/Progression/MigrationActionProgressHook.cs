// <copyright file="MigrationActionProgressHook.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Hooks.Progression
{
    using Tableau.Migration.App.Core.Interfaces;
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
        public MigrationActionProgressHook(IProgressUpdater? progressUpdater = null)
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