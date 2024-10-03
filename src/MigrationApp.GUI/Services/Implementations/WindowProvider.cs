// <copyright file="WindowProvider.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Services.Implementations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MigrationApp.GUI.Services.Interfaces;
using System;

/// <summary>
/// Provides access to the application's main window.
/// </summary>
public class WindowProvider : IWindowProvider
{
    /// <summary>
    /// Retrieves the main window of the application.
    /// </summary>
    /// <returns>The main <see cref="Window"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the application lifetime is not in classic desktop style or the main window is not available.
    /// </exception>
    public Window GetMainWindow()
    {
        // Access the current application
        if (!(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop))
        {
            throw new InvalidOperationException("Application lifetime is not in classic desktop style.");
        }

        // Retrieve the main window
        var mainWindow = desktop.MainWindow;
        if (mainWindow == null)
        {
            throw new InvalidOperationException("Main window is not available.");
        }

        return mainWindow;
    }
}