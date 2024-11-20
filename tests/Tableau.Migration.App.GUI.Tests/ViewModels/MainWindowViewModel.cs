// <copyright file="MainWindowViewModel.cs" company="Salesforce, Inc.">
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;

public class MainWindowViewModelTests
{
    private Mock<IOptions<EmailDomainMappingOptions>> emailDomainOptionsMock;
    private EmailDomainMappingOptions optionsValue;
    private Mock<IOptions<DictionaryUserMappingOptions>> dictionaryUserMappingOptionsMock;
    private Mock<ITableauMigrationService> migrationServiceMock;
    private Mock<IProgressUpdater> progressUpdaterMock;
    private Mock<IProgressMessagePublisher> progressPublisherMock;
    private Mock<IMigrationTimer> migrationTimerMock;
    private Mock<IFilePicker> filePickerMock;
    private Mock<ICsvParser> csvParserMock;
    private Mock<TimersViewModel> timersVMMock;
    private Mock<UserMappingsViewModel> userMappingsVMMock;
    private Mock<UserDomainMappingViewModel> userDomainMappingMock;
    private Mock<UserFileMappingsViewModel> fileMappingMock;
    private MainWindowViewModel viewModel;

    public MainWindowViewModelTests()
    {
        this.emailDomainOptionsMock = new Mock<IOptions<EmailDomainMappingOptions>>();
        this.optionsValue = new EmailDomainMappingOptions();
        this.emailDomainOptionsMock.Setup(o => o.Value).Returns(this.optionsValue);

        this.dictionaryUserMappingOptionsMock = new Mock<IOptions<DictionaryUserMappingOptions>>();
        this.migrationServiceMock = new Mock<ITableauMigrationService>();
        this.progressUpdaterMock = new Mock<IProgressUpdater>();
        this.progressPublisherMock = new Mock<IProgressMessagePublisher>();
        this.migrationTimerMock = new Mock<IMigrationTimer>();
        this.filePickerMock = new Mock<IFilePicker>();
        this.csvParserMock = new Mock<ICsvParser>();
        var migrationTimerMock = new Mock<IMigrationTimer>();
        var timerLoggerMock = new Mock<ILogger<TimersViewModel>>();
        this.timersVMMock = new Mock<TimersViewModel>(
            migrationTimerMock.Object,
            this.progressUpdaterMock.Object,
            timerLoggerMock.Object);
        this.userDomainMappingMock = new Mock<UserDomainMappingViewModel>(this.emailDomainOptionsMock.Object);
        this.fileMappingMock = new Mock<UserFileMappingsViewModel>(
            this.dictionaryUserMappingOptionsMock.Object,
            this.filePickerMock.Object,
            this.csvParserMock.Object);
        this.userMappingsVMMock = new Mock<UserMappingsViewModel>(
            this.userDomainMappingMock.Object,
            this.fileMappingMock.Object);
        this.viewModel = new MainWindowViewModel(
            this.migrationServiceMock.Object,
            this.progressUpdaterMock.Object,
            this.progressPublisherMock.Object,
            this.migrationTimerMock.Object,
            this.timersVMMock.Object,
            this.userMappingsVMMock.Object);
    }

    [AvaloniaFact]
    public void Constructor_initial_state()
    {
        Assert.NotNull(this.viewModel.ServerCredentialsVM);
        Assert.NotNull(this.viewModel.CloudCredentialsVM);
        Assert.False(this.viewModel.IsMigrating);
        Assert.False(this.viewModel.IsNotificationVisible);
        Assert.Empty(this.viewModel.NotificationMessage);
    }

    [AvaloniaFact]
    public async Task SaveManifestIfRequiredAsync_Skip_If_nullAsync()
    {
        await this.viewModel.SaveManifestIfRequiredAsync(null);
        Assert.Equal(" Manifest was not saved.", this.viewModel.NotificationMessage);
    }

    [AvaloniaFact]
    public async void SaveManifestIfRequiredAsync_with_path_succeed()
    {
        this.migrationServiceMock.Setup(ms => ms.SaveManifestAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        await this.viewModel.SaveManifestIfRequiredAsync("somepath");
        Assert.Equal(" Manifest saved.", this.viewModel.NotificationMessage);
    }

    [AvaloniaFact]
    public async void SaveManifestIfRequiredAsync_with_path_failed()
    {
        this.migrationServiceMock.Setup(ms => ms.SaveManifestAsync(It.IsAny<string>())).Returns(Task.FromResult(false));
        await this.viewModel.SaveManifestIfRequiredAsync("somepath");
        Assert.Equal(" Failed to save manifest.", this.viewModel.NotificationMessage);
    }
}