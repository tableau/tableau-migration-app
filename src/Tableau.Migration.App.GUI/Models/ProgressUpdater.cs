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

namespace Tableau.Migration.App.GUI.Models;

using System;
using System.Collections.Generic;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;

/// <summary>
/// Class to track ongoing progress of a migration.
/// </summary>
public class ProgressUpdater : IProgressUpdater
{
    private int currentMigrationStateIndex = -1;
    private List<string> actions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressUpdater" /> class.
    /// </summary>
    public ProgressUpdater()
    {
        this.actions = MigrationActions.Actions;
    }

    /// <summary>
    /// Occurs when changes occur that affect the action of the current migration.
    /// </summary>
    public event EventHandler? OnProgressChanged;

    /// <summary>
    /// Gets the total number of migration states available.
    /// </summary>
    public static int NumMigrationStates { get; } = MigrationActions.Actions.Count;

    /// <summary>
    /// Gets or sets the current migration state index.
    /// </summary>
    public int CurrentMigrationStateIndex
    {
        get => this.currentMigrationStateIndex;
        set
        {
            if (this.currentMigrationStateIndex != value && value <= this.actions.Count)
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
        get => this.CurrentMigrationStateIndex < 0
            || this.CurrentMigrationStateIndex >= this.actions.Count
            ? string.Empty
            : this.actions[this.CurrentMigrationStateIndex];
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