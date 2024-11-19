// <copyright file="UserMappingsViewModel.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Tests.ViewModels;
using Avalonia.Headless.XUnit;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class UserMappingsViewModelTests
{
    [AvaloniaFact]
    public void ValidateAll_Should_Validate_UserDomainMappingVM_And_UserFileMappingsVM()
    {
        var mockEmailMappingOptions = new Mock<IOptions<EmailDomainMappingOptions>>();
        var mockUserDomainMappingVM = new Mock<UserDomainMappingViewModel>(mockEmailMappingOptions.Object);

        var mockDictUserMappingOptions = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var mockFilePicker = new Mock<IFilePicker>();
        var mockCsvParser = new Mock<ICsvParser>();
        var mockUserFileMappingVM = new Mock<UserFileMappingsViewModel>(mockDictUserMappingOptions.Object, mockFilePicker.Object, mockCsvParser.Object);
        var viewModel = new UserMappingsViewModel(mockUserDomainMappingVM.Object, mockUserFileMappingVM.Object);

        viewModel.ValidateAll();

        Mock.Get(viewModel.UserDomainMappingVM).Verify(vm => vm.ValidateAll(), Times.Once);
        Mock.Get(viewModel.UserFileMappingsVM).Verify(vm => vm.ValidateAll(), Times.Once);
    }

    [AvaloniaFact]
    public void GetErrorCount_Should_Return_Sum_Of_All_Errors()
    {
        var mockEmailMappingOptions = new Mock<IOptions<EmailDomainMappingOptions>>();
        var mockUserDomainMappingVM = new Mock<UserDomainMappingViewModel>(mockEmailMappingOptions.Object);

        var mockDictUserMappingOptions = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var mockFilePicker = new Mock<IFilePicker>();
        var mockCsvParser = new Mock<ICsvParser>();
        var mockUserFileMappingVM = new Mock<UserFileMappingsViewModel>(mockDictUserMappingOptions.Object, mockFilePicker.Object, mockCsvParser.Object);
        var viewModel = new UserMappingsViewModel(mockUserDomainMappingVM.Object, mockUserFileMappingVM.Object);

        mockUserDomainMappingVM.Setup(vm => vm.GetErrorCount()).Returns(2);
        mockUserFileMappingVM.Setup(vm => vm.GetErrorCount()).Returns(3);

        var errorCount = viewModel.GetErrorCount();

        Assert.Equal(5, errorCount);
    }

    [AvaloniaFact]
    public void GetErrors_Should_Return_Combined_Errors_From_All_ViewModels()
    {
        var mockEmailMappingOptions = new Mock<IOptions<EmailDomainMappingOptions>>();
        var mockUserDomainMappingVM = new Mock<UserDomainMappingViewModel>(mockEmailMappingOptions.Object);

        var mockDictUserMappingOptions = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var mockFilePicker = new Mock<IFilePicker>();
        var mockCsvParser = new Mock<ICsvParser>();
        var mockUserFileMappingVM = new Mock<UserFileMappingsViewModel>(mockDictUserMappingOptions.Object, mockFilePicker.Object, mockCsvParser.Object);
        var viewModel = new UserMappingsViewModel(mockUserDomainMappingVM.Object, mockUserFileMappingVM.Object);

        var userFileErrors = new List<string> { "FileError1", "FileError2" };
        var userDomainErrors = new List<string> { "DomainError1" };

        Mock.Get(viewModel.UserDomainMappingVM).Setup(vm => vm.GetErrors(It.IsAny<string>())).Returns(userDomainErrors);
        Mock.Get(viewModel.UserFileMappingsVM).Setup(vm => vm.GetErrors(It.IsAny<string>())).Returns(userFileErrors);

        var errors = viewModel.GetErrors(null).ToList();

        Assert.Contains("FileError1", errors);
        Assert.Contains("FileError2", errors);
        Assert.Contains("DomainError1", errors);
    }
}