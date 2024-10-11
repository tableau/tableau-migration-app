// <copyright file="ProgressMessagePublisher.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Models;

using MigrationApp.Core.Entities;
using MigrationApp.Core.Interfaces;
using System;
using Tableau.Migration.Engine.Manifest;
using static MigrationApp.Core.Interfaces.IProgressMessagePublisher;

/// <inheritdoc/>
public class ProgressMessagePublisher : IProgressMessagePublisher
{
    /// <inheritdoc/>
    public event Action<ProgressEventArgs>? OnProgressMessage;

    /// <inheritdoc />
    public void PublishProgressMessage(string action, string message)
    {
        ProgressEventArgs progressMessage = new ProgressEventArgs(action, message);
        this.OnProgressMessage?.Invoke(progressMessage);
        return;
    }
}