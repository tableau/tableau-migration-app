// <copyright file="FilePicker.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.GUI.Services.Implementations;
using Avalonia.Platform.Storage;
using MigrationApp.GUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Service to open up a file picker dialog window.
/// </summary>
public class FilePicker : IFilePicker
{
    private readonly IWindowProvider windowProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePicker"/> class.
    /// </summary>
    /// <param name="windowProvider">The window provider to access the main window.</param>
    public FilePicker(IWindowProvider windowProvider)
    {
        this.windowProvider = windowProvider ?? throw new ArgumentNullException(nameof(windowProvider));
    }

    /// <inheritdoc/>
    public async Task<IStorageFile?> OpenFilePickerAsync(string title, bool allowMultiple = false, string initialDirectory = "")
    {
        var mainWindow = this.windowProvider.GetMainWindow();
        var storageProvider = mainWindow.StorageProvider;
        if (storageProvider == null)
        {
            throw new InvalidOperationException("StorageProvider is not available.");
        }

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("CSV Files")
                {
                    Patterns = new[] { "*.csv" },
                },
                new FilePickerFileType("All Files")
                {
                    Patterns = new[] { "*" },
                },
            },
        };

        var files = await storageProvider.OpenFilePickerAsync(options);
        return files?.Count > 0 ? files[0] : null;
    }
}