// <copyright file="BatchMigrationCompletedProgressHook.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Hooks.Progression;

using Microsoft.Extensions.Logging;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Interfaces;
using Tableau.Migration;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using static MigrationApp.Core.Interfaces.IProgressMessagePublisher;

/// <summary>
/// Batch Migration Hook to broadcast partial migration progress information.
/// </summary>
/// <typeparam name="T">The type of the content reference being migrated.</typeparam>
public class BatchMigrationCompletedProgressHook<T> :
    IContentBatchMigrationCompletedHook<T>
    where T : IContentReference
{
    private const string Separator = "-------------------";
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
        foreach (var result in ctx.ItemResults)
        {
            this.ProcessManifestEntry(result, messageList);
        }

        messageList.Add(BatchMigrationCompletedProgressHook<T>.Separator);

        // Join the messages together to be broadcasted
        string progressMessage = string.Join("\n", messageList);

        // Publish and log the message
        this.publisher?.PublishProgressMessage(
            MigrationActions.ActionNameMapping[typeof(T)],
            progressMessage);

        this.logger.LogInformation(
            "Published progress message for {type}:\n {message}",
            MigrationActions.ActionNameMapping[typeof(T)],
            progressMessage);

        return Task.FromResult<IContentBatchMigrationResult<T>?>(ctx);
    }

    private string ProcessManifestEntry(IContentItemMigrationResult<T> result, List<string> messageList)
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

        return string.Empty;
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