// <copyright file="ProgressEventArgs.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Entities;
using System;
using System.Collections.Generic;

/// <summary>
/// Definition of migration progress event messages.
/// </summary>
public class ProgressEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressEventArgs" /> class.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="message">The message.</param>
    public ProgressEventArgs(string action, string message)
    {
        this.Message = message;
        this.Action = action;
    }

    /// <summary>
    /// Gets something.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets something.
    /// </summary>
    public string Action { get; }
}