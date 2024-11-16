// <copyright file="TimerDisplayViewModelTests.cs" company="Salesforce, Inc.">
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

using Moq;
using System;
using System.Threading;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;
using Xunit.Abstractions;

public class TimerDisplayViewModelTests
{
    private readonly Mock<IProgressTimerController> timerControllerMock;
    private readonly Mock<IProgressUpdater> progressUpdaterMock;
    private readonly ITestOutputHelper output;

    public TimerDisplayViewModelTests(ITestOutputHelper output)
    {
        this.timerControllerMock = new Mock<IProgressTimerController>();
        this.progressUpdaterMock = new Mock<IProgressUpdater>();
        this.output = output;
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var viewModel = new TimerDisplayViewModel(this.timerControllerMock.Object, this.progressUpdaterMock.Object);

        Assert.NotNull(viewModel);
        Assert.Equal("00:00:00", viewModel.TotalElapsedTime);
        Assert.Equal("00:00:00", viewModel.ActionElapsedTime);

        this.timerControllerMock.VerifyAdd(tc => tc.OnMigrationStarted += It.IsAny<Action>(), Times.Once);
        this.timerControllerMock.VerifyAdd(tc => tc.OnMigrationCompleted += It.IsAny<Action>(), Times.Once);
    }

    [Fact]
    public void StartTotalTimer_ShouldUpdateTotalElapsedTime()
    {
        var viewModel = new TimerDisplayViewModel(this.timerControllerMock.Object, this.progressUpdaterMock.Object);

        viewModel.StartTotalTimer();
        Thread.Sleep(1000);

        Assert.NotEqual("00:00:00", viewModel.TotalElapsedTime);
    }

    [Fact]
    public void StopTotalTimer_ShouldResetTotalElapsedTime()
    {
        var viewModel = new TimerDisplayViewModel(this.timerControllerMock.Object, this.progressUpdaterMock.Object);

        viewModel.StartTotalTimer();
        Thread.Sleep(2000);
        Assert.True(TimeSpan.Parse(viewModel.TotalElapsedTime).TotalSeconds > 0);

        viewModel.StopTotalTimer();
    }

    [Fact]
    public void StopActionTimer_ShouldStopActionElapsedTime()
    {
        var viewModel = new TimerDisplayViewModel(this.timerControllerMock.Object, this.progressUpdaterMock.Object);

        viewModel.StartActionTimer();
        Thread.Sleep(1000);
        viewModel.StopActionTimer();

        var elapsedTime = viewModel.ActionElapsedTime;
        Thread.Sleep(500);

        Assert.Equal(elapsedTime, viewModel.ActionElapsedTime);
    }

    [Fact]
    public void ActionText_ShouldUpdateWithProgressUpdater()
    {
        this.progressUpdaterMock.Setup(p => p.CurrentMigrationMessage).Returns("Sample Action");
        var viewModel = new TimerDisplayViewModel(this.timerControllerMock.Object, this.progressUpdaterMock.Object);

        Assert.Equal("Migrating Sample Action:", viewModel.ActionText);
    }
}
