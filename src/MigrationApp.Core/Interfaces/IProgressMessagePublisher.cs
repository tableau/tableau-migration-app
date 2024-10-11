// <copyright file="IProgressMessagePublisher.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Interfaces;
using MigrationApp.Core.Entities;
using System;
using System.Collections.Generic;
using Tableau.Migration.Engine.Manifest;

/// <summary>
/// Class to publish a progress messages events.
/// </summary>
public interface IProgressMessagePublisher
{
    /// <summary>
    /// Message event to notify of progress updates.
    /// </summary>
    event Action<ProgressEventArgs> OnProgressMessage;

    /// <summary>
    /// An enumeration of the various message statuses.
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// Pending message status.
        /// </summary>
        Pending,

        /// <summary>
        /// Skipped message status.
        /// </summary>
        Skipped,

        /// <summary>
        /// Successful message status.
        /// </summary>
        Successful,

        /// <summary>
        /// Error message status.
        /// </summary>
        Error,
    }

    /// <summary>
    /// Returns the appropriate status icon for the progress messages based on the provided status.
    /// </summary>
    /// <param name="status">The status of the message, represented as a <see cref="MessageStatus"/> enumeration.</param>
    /// <returns>
    /// A string containing the Unicode character that represents the status:
    /// \U0001F7E1 for "Pending",
    /// \U0001F535 for "Skipped",
    /// \U0001F534 for "Error",
    /// \U0001F7E2 for "Successful",
    /// and \U0001F7E3 for any unknown status.
    /// </returns>
    public static string GetStatusIcon(MessageStatus status)
    {
        string statusIcon;
        switch (status)
        {
            case MessageStatus.Pending:
                statusIcon = "\U0001F7E1"; // 🟡
                break;
            case MessageStatus.Skipped:
                statusIcon = "\U0001F535"; // 🔵
                break;
            case MessageStatus.Error:
                statusIcon = "\U0001F534"; // 🔴
                break;
            case MessageStatus.Successful:
                statusIcon = "\U0001F7E2"; // 🟢
                break;
            default:
                statusIcon = "\U0001F7E3"; // 🟣 Unknown status
                break;
        }

        return statusIcon;
    }

    /// <summary>
    /// Publish progress message of current Migration Action with its corresponding status messages.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="message">The message.</param>
    void PublishProgressMessage(string action, string message);
}