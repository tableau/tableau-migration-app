// <copyright file="MainWindowViewModelTests.cs" company="Salesforce, Inc.">
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

using Microsoft.Extensions.Options;
using Moq;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void CloudUserDomain_ShouldUpdateEmailDomainOptions()
    {
        var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        var optionsValue = new EmailDomainMappingOptions();
        emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

        var dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var migrationServiceMock = new Mock<ITableauMigrationService>();
        var progressUpdaterMock = new Mock<IProgressUpdater>();
        var progressPublisherMock = new Mock<IProgressMessagePublisher>();
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();
        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            progressPublisherMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain = "testdomain.com";

        Assert.Equal("testdomain.com", optionsValue.EmailDomain);
    }

    [Fact]
    public void CloudUserDomain_ShouldTriggerValidationErrors_WhenEmpty()
    {
        var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        var optionsValue = new EmailDomainMappingOptions();
        emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

        var dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var migrationServiceMock = new Mock<ITableauMigrationService>();
        var progressUpdaterMock = new Mock<ProgressUpdater>();
        var progressPublisherMock = new Mock<IProgressMessagePublisher>();
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            progressPublisherMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain = string.Empty;

        Assert.NotEmpty(
            viewModel
            .UserMappingsVM
            .GetErrors(
                nameof(viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain)));
    }

    [Fact]
    public void CloudUserDomain_ShouldTriggerValidationErrors_WhenInvalidDomain()
    {
        var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        var optionsValue = new EmailDomainMappingOptions();
        emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

        var dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var migrationServiceMock = new Mock<ITableauMigrationService>();
        var progressUpdaterMock = new Mock<ProgressUpdater>();
        var progressPublisherMock = new Mock<IProgressMessagePublisher>();
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            progressPublisherMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain = "invalid_domain";

        Assert.NotEmpty(
            viewModel
            .UserMappingsVM
            .GetErrors(
                nameof(viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain)));
    }

    [Fact]
    public void CloudUserDomain_ShouldTriggerValidationErrors_WhenInvalidDomain_2()
    {
        var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        var optionsValue = new EmailDomainMappingOptions();
        emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

        var dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var migrationServiceMock = new Mock<ITableauMigrationService>();
        var progressUpdaterMock = new Mock<ProgressUpdater>();
        var progressPublisherMock = new Mock<IProgressMessagePublisher>();
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            progressPublisherMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain = "invalid_domain.c";

        Assert.NotEmpty(
            viewModel
            .UserMappingsVM
            .GetErrors(
                nameof(viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain)));
    }

    [Fact]
    public void CloudUserDomain_ShouldNotTriggerValidationErrors_WhenValidDomain()
    {
        var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        var optionsValue = new EmailDomainMappingOptions();
        emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

        var dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var migrationServiceMock = new Mock<ITableauMigrationService>();
        var progressUpdaterMock = new Mock<ProgressUpdater>();
        var progressPublisherMock = new Mock<IProgressMessagePublisher>();
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            progressPublisherMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain = "valid-domain.com";

        Assert.Empty(
            viewModel
            .UserMappingsVM
            .GetErrors(
                nameof(viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain)));
    }

    [Fact]
    public void CloudUserDomain_ShouldNotTriggerValidationErrors_WhenValidDomain_2()
    {
        var emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        var optionsValue = new EmailDomainMappingOptions();
        emailDomainOptionsMock.Setup(o => o.Value).Returns(optionsValue);

        var dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        var migrationServiceMock = new Mock<ITableauMigrationService>();
        var progressUpdaterMock = new Mock<ProgressUpdater>();
        var progressPublisherMock = new Mock<IProgressMessagePublisher>();
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            progressPublisherMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain = "valid.domain.com";

        Assert.Empty(
            viewModel
            .UserMappingsVM
            .GetErrors(
                nameof(viewModel.UserMappingsVM.UserDomainMappingVM.CloudUserDomain)));
    }
}