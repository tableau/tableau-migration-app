// <copyright file="IMigrationTimer.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.Core.Entities;

/// <summary>
/// Migration Timer Interface.
/// </summary>
public interface IMigrationTimer
{
    /// <summary>
    /// Gets the total time spent on the Migration. If migration is ongoing, it will give the total elapsed time so far.
    /// </summary>
    /// <returns>A string representing the total time in `hh:mm:ss` format.</returns>
    string GetTotalMigrationTime { get; }

    /// <summary>
    /// Gets the total time spent for migration of a specific action. Actions that were skipped or have not happened yet will be 0.
    /// </summary>
    /// <param name="actionName">The Action to query.</param>
    /// <returns>The string representing teh total time in `hh:mm:ss` format.</returns>
    string GetMigrationActionTime(string actionName);

    /// <summary>
    /// Updates the migration timings based on the new event.
    /// </summary>
    /// <param name="timerEvent">The timer event.</param>
    /// <param name="migrationAction">The migration action to update.</param>
    void UpdateMigrationTimes(MigrationTimerEventType timerEvent, string migrationAction);

    /// <summary>
    /// Updates the migration timings based on the new event.
    /// </summary>
    /// <param name="timerEvent">The timer event.</param>
    void UpdateMigrationTimes(MigrationTimerEventType timerEvent);

    /// <summary>
    /// Resets the migration timers.
    /// </summary>
    void Reset();
}