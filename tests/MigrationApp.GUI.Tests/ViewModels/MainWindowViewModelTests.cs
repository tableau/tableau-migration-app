// <copyright file="MainWindowViewModelTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Tests.ViewModels;

using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Interfaces;
using MigrationApp.GUI.Models;
using MigrationApp.GUI.Services.Interfaces;
using MigrationApp.GUI.ViewModels;
using Moq;

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
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();
        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.CloudUserDomain = "testdomain.com";

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
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.CloudUserDomain = string.Empty;

        Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.CloudUserDomain)));
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
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.CloudUserDomain = "invalid_domain";

        Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.CloudUserDomain)));
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
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.CloudUserDomain = "invalid_domain.c";

        Assert.NotEmpty(viewModel.GetErrors(nameof(viewModel.CloudUserDomain)));
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
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.CloudUserDomain = "valid-domain.com";

        Assert.Empty(viewModel.GetErrors(nameof(viewModel.CloudUserDomain)));
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
        var filePickerMock = new Mock<IFilePicker>();
        var csvParserMock = new Mock<ICsvParser>();

        var viewModel = new MainWindowViewModel(
            migrationServiceMock.Object,
            emailDomainOptionsMock.Object,
            dictionaryUserMappingOptionsMock.Object,
            progressUpdaterMock.Object,
            filePickerMock.Object,
            csvParserMock.Object);

        viewModel.CloudUserDomain = "valid.domain.com";

        Assert.Empty(viewModel.GetErrors(nameof(viewModel.CloudUserDomain)));
    }
}