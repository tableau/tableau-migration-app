// <copyright file="IProgressTimerController.cs" company="Salesforce, Inc.">
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

using System;

/// <summary>
/// Interface to publish a progress timer of events.
/// </summary>
public interface IProgressTimerController
{
    /// <summary>
    /// Message event to notify of progress updates.
    /// </summary>
    event Action OnMigrationStarted;

    /// <summary>
    /// Message event to notify of progress updates.
    /// </summary>
    event Action OnMigrationCompleted;

    /// <summary>
    /// Starts the total migration timer if it hasn't started yet,
    /// and resets the action timer to track the time for a new action.
    /// </summary>
    void StartMigrationTimer();

    /// <summary>
    /// Stops the total migration timer and the action timer for the last action,
    /// then publishes the provided migration result message along with the total elapsed time.
    /// </summary>
    void CompleteMigration();
}
