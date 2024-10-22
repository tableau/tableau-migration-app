// <copyright file="WindowProvider.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Services.Implementations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using Tableau.Migration.App.GUI.Services.Interfaces;

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