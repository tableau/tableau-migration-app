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

namespace FilePickerTests;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Platform.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tableau.Migration.App.GUI.Services.Implementations;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Xunit;

public class FilePickerTests
{
    [AvaloniaFact]
    public void FilePicker()
    {
        var mockWindowProvider = new Mock<IWindowProvider>();
        var fp = new FilePicker(mockWindowProvider.Object);
        Assert.NotNull(fp);
    }

    [AvaloniaFact(Skip = "Skipped until StorageProvider setting logic is abstracted.")]
    public async Task OpenFilePickerAsync_ReturnsSelectedFile()
    {
        var mockWindowProvider = new Mock<IWindowProvider>();
        var mockWindow = new Mock<Window>();
        var mockStorageProvider = new Mock<IStorageProvider>();
        var mockStorageFile = new Mock<IStorageFile>();

        mockWindowProvider.Setup(w => w.GetMainWindow()).Returns(mockWindow.Object);
        mockWindow.SetupGet(
            sp => sp.StorageProvider).Returns(mockStorageProvider.Object);

        // Being unable to mock the main window's StorageProvider will make the
        // function short citcuit out. The function will need to be refactored
        // to support testing properly.
        mockStorageProvider.Setup(s => s.OpenFilePickerAsync(It.IsAny<FilePickerOpenOptions>())).ReturnsAsync(new List<IStorageFile> { mockStorageFile.Object });

        var filePicker = new FilePicker(mockWindowProvider.Object);

        var result = await filePicker.OpenFilePickerAsync("Select File");

        Assert.NotNull(result);
        Assert.Equal(mockStorageFile.Object, result);
    }
}