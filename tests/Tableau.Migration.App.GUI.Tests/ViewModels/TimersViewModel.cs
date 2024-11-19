// <copyright file="TimersViewModel.cs" company="Salesforce, Inc.">
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

namespace TimersViewModelTests;

using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;

public class TimersViewModelTests
{
    private readonly Mock<IMigrationTimer> migrationTimerMock;
    private readonly Mock<IProgressUpdater> progressUpdaterMock;
    private readonly Mock<ILogger<TimersViewModel>> loggerMock;
    private readonly TimersViewModel viewModel;

    public TimersViewModelTests()
    {
        this.migrationTimerMock = new Mock<IMigrationTimer>();
        this.progressUpdaterMock = new Mock<IProgressUpdater>();
        this.loggerMock = new Mock<ILogger<TimersViewModel>>();

        this.viewModel = new TimersViewModel(this.migrationTimerMock.Object, this.progressUpdaterMock.Object, this.loggerMock.Object);
    }

    [AvaloniaFact]
    public async void Start_ShouldInitializeTotalElapsedTimeAndStartTimer()
    {
        this.migrationTimerMock.Setup(x => x.GetTotalMigrationTime).Returns("12h:34m:56s");
        this.migrationTimerMock.Setup(x => x.GetMigrationActionTime(It.IsAny<string>())).Returns("65h:43m:21s");
        this.viewModel.Start();

        Assert.Equal(string.Empty, this.viewModel.TotalElapsedTime);
        Assert.False(this.viewModel.ShowActionTimer);

        // After 1s, the dispatcher should have run and updated the values
        await Task.Delay(1500);
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Background);

        Assert.Equal("12h:34m:56s", this.viewModel.TotalElapsedTime);
        Assert.True(this.viewModel.ShowActionTimer);
    }

    [AvaloniaFact]
    public void Stop_ShouldStopTimerAndSetShowActionTimerToFalse()
    {
        this.viewModel.Start();
        this.viewModel.Stop();

        Assert.False(this.viewModel.ShowActionTimer);
    }

    [AvaloniaFact]
    public async void UpdateTimers_ShouldUpdatePropertiesCorrectly()
    {
        this.migrationTimerMock.Setup(x => x.GetTotalMigrationTime).Returns("01:00:00");
        this.migrationTimerMock.Setup(x => x.GetMigrationActionTime(It.IsAny<string>())).Returns("00:05:00");
        this.progressUpdaterMock.Setup(x => x.CurrentMigrationStateName).Returns("Action 1");

        await Dispatcher.UIThread.InvokeAsync(() => this.viewModel.Start());
        await Task.Delay(1500);
        Dispatcher.UIThread.RunJobs(DispatcherPriority.Normal);

        Assert.Equal("01:00:00", this.viewModel.TotalElapsedTime);
        Assert.Equal("00:05:00", this.viewModel.CurrentActionTime);
        Assert.Equal("Action 1: ", this.viewModel.CurrentActionLabel);
    }

    [AvaloniaFact]
    public void Stop_HideActionTimer()
    {
        this.viewModel.Stop();
        Assert.False(this.viewModel.ShowActionTimer);
    }
}