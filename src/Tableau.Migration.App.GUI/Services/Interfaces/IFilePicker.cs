// <copyright file="IFilePicker.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Services.Interfaces;

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