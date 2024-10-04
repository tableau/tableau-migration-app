// <copyright file="IFilePicker.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Services.Interfaces;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

/// <summary>
/// Main interface for a file picker dialog window.
/// </summary>
public interface IFilePicker
{
    /// <summary>
    /// Opens a file picker dialog.
    /// </summary>
    /// <param name="title">The title of the file picker dialog.</param>
    /// <param name="allowMultiple">Determines whether multiple files can be selected.</param>
    /// <param name="initialDirectory">The initial directory displayed by the file picker.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the selected file, or <c>null</c> if no file was selected.</returns>
    Task<IStorageFile?> OpenFilePickerAsync(string title, bool allowMultiple = false, string initialDirectory = "");
}