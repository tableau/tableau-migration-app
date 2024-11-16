// <copyright file="ProgressMessagePublisher.cs" company="Salesforce, Inc.">
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
using System.Diagnostics;
using System.Text;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;

/// <inheritdoc/>
public class ProgressMessagePublisher : IProgressMessagePublisher
{
    private const string Separator = "-------------------";
    private readonly Stopwatch totalStopwatch;
    private readonly Stopwatch actionStopwatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressMessagePublisher"/> class.
    /// </summary>
    public ProgressMessagePublisher()
    {
        this.totalStopwatch = new Stopwatch();
        this.actionStopwatch = new Stopwatch();
    }

    /// <inheritdoc/>
    public event Action<ProgressEventArgs>? OnProgressMessage;

    /// <inheritdoc/>
    public void StartMigrationTimer()
    {
        this.totalStopwatch.Restart();
        this.actionStopwatch.Restart();
    }

    /// <inheritdoc/>
    public void CompleteActionWithProgressMessage(string action, string progressMessage)
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(progressMessage);

        if (this.actionStopwatch.IsRunning)
        {
            this.actionStopwatch.Stop();
            messageBuilder.AppendLine($"Elapsed time: {this.actionStopwatch.Elapsed}");
        }

        messageBuilder.AppendLine(ProgressMessagePublisher.Separator);
        this.PublishProgressMessage(action, messageBuilder.ToString());

        // Restart the timer for the next action
        this.actionStopwatch.Restart();
    }

    /// <inheritdoc/>
    public void CompleteMigrationWithResultMessage(string migrationResultText, string resultMessage)
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(resultMessage);

        if (this.totalStopwatch.IsRunning)
        {
            this.totalStopwatch.Stop();
            messageBuilder.AppendLine($"Total elapsed time for migration: {this.totalStopwatch.Elapsed}");
        }

        this.PublishProgressMessage(migrationResultText, messageBuilder.ToString());
    }

    /// <summary>
    /// Publishes a progress message for a specific action or migration status.
    /// </summary>
    /// <param name="header">The action or migration status.</param>
    /// <param name="message">The message to be published.</param>
    private void PublishProgressMessage(string header, string message)
    {
        ProgressEventArgs progressMessage = new ProgressEventArgs(header, message);
        this.OnProgressMessage?.Invoke(progressMessage);
    }
}