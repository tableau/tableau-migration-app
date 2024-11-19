// <copyright file="MigrationTimerEvent.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Entities;
using System;

/// <summary>
/// Possible value types for Migration Timer Events.
/// </summary>
public enum MigrationTimerEventType
{
    /// <summary>
    /// Event fired when Migration has started.
    /// </summary>
    MigrationStarted,

    /// <summary>
    /// Event fired when Migration has either completed or failed.
    /// </summary>
    Migrationfinished,

    /// <summary>
    /// Event fired when a Migration Action has completed.
    /// </summary>
    MigrationActionCompleted,
}

/// <summary>
/// Migration Time events to be triggered.
/// </summary>
public class MigrationTimerEvent : EventArgs
{
    private DateTime migrationStartTime;
    private Dictionary<string, DateTime> actionStartTimes;
    private Dictionary<string, DateTime> actionStopTimes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationTimerEvent" /> class.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    public MigrationTimerEvent(MigrationTimerEventType eventType)
    {
        this.EventType = eventType;
        this.migrationStartTime = DateTime.Now;
        this.actionStartTimes = new Dictionary<string, DateTime>();
        this.actionStopTimes = new Dictionary<string, DateTime>();
    }

    /// <summary>
    /// Gets the Migration event type.
    /// </summary>
    public MigrationTimerEventType EventType { get; }
}