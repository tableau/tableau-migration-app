// <copyright file="FilePicker.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Services.Implementations;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MigrationApp.GUI.Services.Implementations;
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