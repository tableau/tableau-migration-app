// <copyright file="MigrationTimer.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Models;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;

/// <inheritdoc/>
public class MigrationTimer : IMigrationTimer
{
    private ILogger<MigrationTimer>? logger;
    private DateTime? startMigrationTime;
    private DateTime? stopMigrationTime;
    private Dictionary<
        string /* Action Name */,
        DateTime /* Start Time */> startActionTimes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationTimer"/> class.
    /// Initializes an new instance of the <see cref="MigrationTimer" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public MigrationTimer(ILogger<MigrationTimer>? logger = null)
    {
        this.startActionTimes = new Dictionary<string, DateTime>();
        this.logger = logger;
    }

    /// <inheritdoc/>
    public string GetTotalMigrationTime
    {
        get
        {
            if (this.startMigrationTime == null)
            {
                return string.Empty;
            }

            DateTime now = DateTime.Now;
            if (this.stopMigrationTime != null)
            {
                now = this.stopMigrationTime.Value;
            }

            TimeSpan timeSpan = now - this.startMigrationTime.Value;
            return this.FormatTime(timeSpan);
        }
    }

    /// <inheritdoc/>
    public string GetMigrationActionTime(string currentAction)
    {
        if (!this.startActionTimes.ContainsKey(currentAction))
        {
            return string.Empty;
        }

        DateTime start = this.startActionTimes[currentAction];
        int actionIndex = MigrationActions.GetActionIndex(currentAction);
        bool lastAction = actionIndex == MigrationActions.Actions.Count - 1;
        DateTime stop = DateTime.Now;

        if (!lastAction && this.startActionTimes.ContainsKey(MigrationActions.Actions[actionIndex + 1]))
        {
            stop = this.startActionTimes[MigrationActions.Actions[actionIndex + 1]];
        }

        TimeSpan diff = stop - start;
        return this.FormatTime(diff);
    }

    /// <inheritdoc/>
    public void UpdateMigrationTimes(
        MigrationTimerEventType timerEvent)
    {
        this.UpdateMigrationTimes(timerEvent, string.Empty);
    }

    /// <inheritdoc/>
    public void UpdateMigrationTimes(
        MigrationTimerEventType timerEvent,
        string migrationAction)
    {
        switch (timerEvent)
        {
            case MigrationTimerEventType.MigrationStarted:
                this.startMigrationTime = DateTime.Now;
                this.logger?.LogInformation($"Migration timer started: {this.startMigrationTime:yyyy-MM-dd HH:mm:ss}");
                break;
            case MigrationTimerEventType.Migrationfinished:
                this.stopMigrationTime = DateTime.Now;
                this.logger?.LogInformation($"Migration timer stopped: {this.stopMigrationTime:yyyy-MM-dd HH:mm:ss}.");
                break;
            case MigrationTimerEventType.MigrationActionCompleted:
                if (string.IsNullOrEmpty(migrationAction))
                {
                    this.logger?.LogError("Attempted to log a completed migration action time without a migration action name");
                    return;
                }

                if (this.startActionTimes.ContainsKey(migrationAction))
                {
                    this.logger?.LogInformation($"Attempted to log a completed migration action time when one already exists: {migrationAction}");
                    return;
                }

                this.startActionTimes[migrationAction] = DateTime.Now;
                this.logger?.LogInformation($"Migration Action timer set for {migrationAction}: {this.startActionTimes[migrationAction]:yyyy-MM-dd HH:mm:ss}.");
                return;
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        this.startMigrationTime = null;
        this.stopMigrationTime = null;
        this.startActionTimes = new Dictionary<string, DateTime>();
    }

    private string FormatTime(TimeSpan timeSpan)
    {
        string formattedTime = timeSpan.Days > 0
            ? $"{timeSpan.Days} days, {timeSpan.Hours:00}h:{timeSpan.Minutes:00}m:{timeSpan.Seconds:00}s"
            : $"{timeSpan.Hours:00}h:{timeSpan.Minutes:00}m:{timeSpan.Seconds:00}s";
        return formattedTime;
    }
}