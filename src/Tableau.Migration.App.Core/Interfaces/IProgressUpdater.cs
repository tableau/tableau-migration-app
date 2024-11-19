// <copyright file="IProgressUpdater.cs" company="Salesforce, Inc.">
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
    /// Updates the visual representation of the migration progress.
    /// </summary>
    void Update();

    /// <summary>
    /// Resets the visual representation of the migration state back to initial states.
    /// </summary>
    void Reset();
}