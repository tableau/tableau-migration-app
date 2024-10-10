// <copyright file="IProgressUpdater.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Interfaces;

/// <summary>
/// Interface for object to manage migration progression.
/// </summary>
public interface IProgressUpdater
{
    /// <summary>
    /// Event to signal for migration state progress updates.
    /// </summary>
    event EventHandler? OnProgressChanged;

    /// <summary>
    /// Gets or Sets a value representing the current Migration state.
    /// </summary>
    int CurrentMigrationStateIndex { get; set; }

    /// <summary>
    /// Gets the name associated with the current migration state.
    /// </summary>
    string CurrentMigrationStateName { get; }

    /// <summary>
    /// Gets the message associated with the current migration state.
    /// </summary>
    string CurrentMigrationMessage { get; }

    /// <summary>
    /// Gets the total number of migration states available.
    /// </summary>
    static int NumMigrationStates { get; }

    /// <summary>
    /// Updates the visual representation of the migration progress.
    /// </summary>
    void Update();

    /// <summary>
    /// Resets the visual representation of the migration state back to initial states.
    /// </summary>
    void Reset();
}