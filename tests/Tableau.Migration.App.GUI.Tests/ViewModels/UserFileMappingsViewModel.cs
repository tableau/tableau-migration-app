// <copyright file="UserFileMappingsViewModel.cs" company="Salesforce, Inc.">
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

namespace UserFileMappingsViewModelTests;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class UserFileMappingsViewModelTests
{
    [AvaloniaFact]
    public void UnLoadUserFile_ClearsLoadedValues()
    {
        var dictionaryOptions = Options.Create(new DictionaryUserMappingOptions { UserMappings = new Dictionary<string, string>() });
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();
        var viewModel = new UserFileMappingsViewModel(dictionaryOptions, filePickerMock.Object, csvParserMock.Object)
        {
            LoadedCSVFilename = "test.csv",
            CSVLoadStatus = "Loaded",
            CSVLoadStatusColor = Brushes.Green,
            IsUserMappingFileLoaded = true,
        };

        viewModel.UnLoadUserFileCommand.Execute(null);

        Assert.Equal("No file loaded.", viewModel.LoadedCSVFilename);
        Assert.Equal(string.Empty, viewModel.CSVLoadStatus);
        Assert.Equal(Brushes.Black, viewModel.CSVLoadStatusColor);
        Assert.False(viewModel.IsUserMappingFileLoaded);
    }

    [AvaloniaFact]
    public async Task LoadUserFile_FileNotFound_SetsErrorStatus()
    {
        var dictionaryOptions = Options.Create(new DictionaryUserMappingOptions { UserMappings = new Dictionary<string, string>() });
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();
        filePickerMock.Setup(fp => fp.OpenFilePickerAsync(
                                 It.IsAny<string>(),
                                 It.IsAny<bool>(),
                                 It.IsAny<string>()))
            .Returns(Task.FromResult<IStorageFile?>(null));
        var viewModel = new UserFileMappingsViewModel(dictionaryOptions, filePickerMock.Object, csvParserMock.Object);

        await viewModel.LoadUserFileCommand.ExecuteAsync(null);

        Assert.False(viewModel.IsUserMappingFileLoaded, "User mappings file should not be considered loaded if the csv file does not exist.");
    }

    [AvaloniaFact]
    public async Task LoadUserFile_InvalidData_SetsErrorStatus()
    {
        var dictionaryOptions = Options.Create(new DictionaryUserMappingOptions { UserMappings = new Dictionary<string, string>() });
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();
        var fileMock = new Mock<IStorageFile>();

        // fileMock.Setup(f => f.TryGetLocalPath()).Returns("invalid.csv");
        fileMock.SetupGet(f => f.Path).Returns(new Uri("file://invalid.csv"));
        fileMock.Setup(f => f.Name).Returns("invalid.csv");
        filePickerMock.Setup(fp => fp.OpenFilePickerAsync(
                                 It.IsAny<string>(),
                                 It.IsAny<bool>(),
                                 It.IsAny<string>()))
            .Returns(Task.FromResult<IStorageFile?>(fileMock.Object));
        csvParserMock.Setup(cp => cp.ParseAsync(It.IsAny<string>())).ThrowsAsync(new InvalidDataException("Invalid CSV format."));
        var viewModel = new UserFileMappingsViewModel(dictionaryOptions, filePickerMock.Object, csvParserMock.Object);

        await viewModel.LoadUserFileCommand.ExecuteAsync(null);

        Assert.False(viewModel.IsUserMappingFileLoaded, "User mapping should not be considered loaded if the CSV could not be parsed.");
    }

    [AvaloniaFact]
    public async Task LoadUserFile_ValidFile_LoadsMappings()
    {
        var userMappings = new Dictionary<string, string> { { "User1", "Mapping1" } };
        var dictionaryOptions = Options.Create(new DictionaryUserMappingOptions { UserMappings = new Dictionary<string, string>() });
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();
        var fileMock = new Mock<IStorageFile>();
        fileMock.SetupGet(f => f.Path).Returns(new Uri("file://valid.csv"));
        fileMock.Setup(f => f.Name).Returns("valid.csv");
        filePickerMock.Setup(fp => fp.OpenFilePickerAsync(
                                 It.IsAny<string>(),
                                 It.IsAny<bool>(),
                                 It.IsAny<string>()))
            .Returns(Task.FromResult<IStorageFile?>(fileMock.Object));
        csvParserMock.Setup(cp => cp.ParseAsync(It.IsAny<string>())).ReturnsAsync(userMappings);
        var viewModel = new UserFileMappingsViewModel(dictionaryOptions, filePickerMock.Object, csvParserMock.Object);

        await viewModel.LoadUserFileCommand.ExecuteAsync(null);

        Assert.Equal("valid.csv", viewModel.LoadedCSVFilename);
        Assert.Equal("1 user mappings loaded.", viewModel.CSVLoadStatus);
        Assert.Equal(Brushes.Black, viewModel.CSVLoadStatusColor);
        Assert.True(viewModel.IsUserMappingFileLoaded);
        Assert.Equal(userMappings, dictionaryOptions.Value.UserMappings);
    }
}