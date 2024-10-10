// <copyright file="IProgressMessagePublisher.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Interfaces;
using MigrationApp.Core.Entities;
using System;
using System.Collections.Generic;

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
    /// Publish progress message of current Migration Action with its corresponding status messages.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="message">The message.</param>
    void PublishProgressMessage(string action, string message);
}