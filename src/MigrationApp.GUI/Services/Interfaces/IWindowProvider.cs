// <copyright file="IWindowProvider.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Services.Interfaces;
using Avalonia.Controls;

/// <summary>
/// Defines a contract for providing access to the application's main window.
/// </summary>
public interface IWindowProvider
{
    /// <summary>
    /// Retrieves the main window of the application.
    /// </summary>
    /// <returns>The main <see cref="Window"/> instance.</returns>
    Window GetMainWindow();
}