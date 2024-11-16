// <copyright file="BatchMigrationCompletedProgressHook.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Hooks.Progression;

using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using static Tableau.Migration.App.Core.Interfaces.IProgressMessagePublisher;

/// <summary>
/// Batch Migration Hook to broadcast partial migration progress information.
/// </summary>
/// <typeparam name="T">The type of the content reference being migrated.</typeparam>
public class BatchMigrationCompletedProgressHook<T> :
    IContentBatchMigrationCompletedHook<T>
    where T : IContentReference
{
    private readonly ILogger<BatchMigrationCompletedProgressHook<T>> logger;
    private readonly IProgressMessagePublisher? publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchMigrationCompletedProgressHook{T}"/> class.
    /// </summary>
    /// <param name="logger">The injected Logging interface.</param>
    /// <param name="publisher">The message publisher to broadcast progress updates.</param>
    public BatchMigrationCompletedProgressHook(
        ILogger<BatchMigrationCompletedProgressHook<T>> logger,
        IProgressMessagePublisher? publisher = null)
    {
        this.logger = logger;
        this.publisher = publisher;
    }

    /// <inheritdoc/>
    public Task<IContentBatchMigrationResult<T>?> ExecuteAsync(IContentBatchMigrationResult<T> ctx, CancellationToken cancel)
    {
        // Build publishing message
        List<string> messageList = new ();
        if (ctx.ItemResults.Count == 0)
        {
            this.logger.LogInformation(
                "No resources found for {type} to be migrated.",
                MigrationActions.GetActionTypeName(typeof(T)));
            return Task.FromResult<IContentBatchMigrationResult<T>?>(ctx);
        }

        foreach (var result in ctx.ItemResults)
        {
            this.ProcessManifestEntry(result, messageList);
        }

        // Join the messages together to be broadcasted
        string progressMessage = string.Join("\n", messageList);

        // Publish and log the message
        this.publisher?.CompleteActionWithProgressMessage(
            MigrationActions.GetActionTypeName(typeof(T)),
            progressMessage);

        this.logger.LogInformation(
            "Published progress message for {type}:\n {message}",
            MigrationActions.GetActionTypeName(typeof(T)),
            progressMessage);

        return Task.FromResult<IContentBatchMigrationResult<T>?>(ctx);
    }

    private void ProcessManifestEntry(IContentItemMigrationResult<T> result, List<string> messageList)
    {
        // Construct a status entry from the manifest entry result
        var statusIcon = IProgressMessagePublisher.GetStatusIcon(this.ConvertToMessageStatus(result.ManifestEntry.Status));
        messageList.Add($"\t {statusIcon} [{result.ManifestEntry.Source.Location.Name}] to [{result.ManifestEntry.MappedLocation.Name}] → {result.ManifestEntry.Status}");
        foreach (var error in result.ManifestEntry.Errors)
        {
            try
            {
                ErrorMessage parsedError = new ErrorMessage(error.Message);
                messageList.Add($"\t\t{parsedError.Detail}");
            }
            catch (Exception)
            {
                messageList.Add($"Could not parse error message: \n{error.Message}");
            }
        }
    }

    private MessageStatus ConvertToMessageStatus(MigrationManifestEntryStatus status)
    {
        switch (status)
        {
            case MigrationManifestEntryStatus.Pending:
                return MessageStatus.Pending;
            case MigrationManifestEntryStatus.Skipped:
                return MessageStatus.Skipped;
            case MigrationManifestEntryStatus.Migrated:
                return MessageStatus.Successful;
            case MigrationManifestEntryStatus.Error:
                return MessageStatus.Error;
            case MigrationManifestEntryStatus.Canceled:
                return MessageStatus.Error;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown status value");
        }
    }
}