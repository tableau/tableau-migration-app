// <copyright file="ProgressUpdater.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.GUI.Models;

using MigrationApp.Core.Entities;
using MigrationApp.Core.Interfaces;
using System;

/// <summary>
/// Class to track ongoing progress of a migration.
/// </summary>
public class ProgressUpdater : IProgressUpdater
{
    private int currentMigrationStateIndex = -1;

    /// <summary>
    /// Occurs when changes occur that affect the action of the current migration.
    /// </summary>
    public event EventHandler? OnProgressChanged;

    /// <summary>
    /// Gets the total number of migration states available.
    /// </summary>
    public static int NumMigrationStates { get; } = MigrationActions.Actions.Length;

    /// <summary>
    /// Gets or sets the current migration state index.
    /// </summary>
    public int CurrentMigrationStateIndex
    {
        get => this.currentMigrationStateIndex;
        set
        {
            if (this.currentMigrationStateIndex != value && value <= MigrationActions.Actions.Length)
            {
                this.currentMigrationStateIndex = value;
                this.OnProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets the name asssociated with the current migration state.
    /// </summary>
    public string CurrentMigrationStateName
    {
        get => this.CurrentMigrationStateIndex < 0 || this.CurrentMigrationStateIndex >= MigrationActions.Actions.Length ?
            string.Empty : MigrationActions.Actions[this.CurrentMigrationStateIndex];
    }

    /// <summary>
    /// Gets the migration message associated with the current migration state.
    /// </summary>
    public string CurrentMigrationMessage
    {
        get
        {
            if (this.CurrentMigrationStateIndex < 0)
            {
                return string.Empty;
            }
            else if (this.CurrentMigrationStateIndex >= NumMigrationStates)
            {
                return "Migration Finished.";
            }

            return $"{this.CurrentMigrationStateName}";
        }
    }

    /// <summary>
    /// Increases the current migration state to the next action.
    /// </summary>
    /// <remarks>
    /// This function exists for increased legibility when ProgressUpdater is nullable.
    /// </remarks>
    public void Update()
    {
        this.CurrentMigrationStateIndex++;
    }

    /// <summary>
    /// Resets the migration progress back to initial state.
    /// </summary>
    public void Reset()
    {
        this.CurrentMigrationStateIndex = -1;
    }
}