// <copyright file="ProgressUpdater.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
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

            return $"Migrating {this.CurrentMigrationStateName}";
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