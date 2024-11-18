// <copyright file="IProgressMessagePublisher.cs" company="Salesforce, Inc.">
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
using Tableau.Migration.App.Core.Entities;

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
    /// <param name="message">The message.</param>
    void PublishProgressMessage(string message);
}