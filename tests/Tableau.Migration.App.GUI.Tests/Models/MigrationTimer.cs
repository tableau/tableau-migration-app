// <copyright file="MigrationTimer.cs" company="Salesforce, Inc.">
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

namespace MigrationTimerTests;

using Avalonia.Headless.XUnit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.GUI.Models;
using Xunit;

public class MigrationTimerTests
{
    [AvaloniaFact]
    public void MigrationTimer_WhenInitialized_StartsWithDefaultValues()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);

        Assert.Equal(string.Empty, migrationTimer.GetTotalMigrationTime);
    }

    [AvaloniaFact]
    public void UpdateMigrationTimes_WhenMigrationStarted_LogsStartTime()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);

        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationStarted);

        Assert.NotEqual(string.Empty, migrationTimer.GetTotalMigrationTime);
    }

    [AvaloniaFact]
    public void UpdateMigrationTimes_WhenMigrationFinished_StoreStopTime()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);

        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationStarted);
        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.Migrationfinished);

        Assert.NotEqual(string.Empty, migrationTimer.GetTotalMigrationTime);
    }

    [AvaloniaFact]
    public void UpdateMigrationTimes_WhenMigrationActionCompleted_storeActionTime()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);
        string actionName = "TestAction";

        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationActionCompleted, actionName);

        Assert.NotEqual(string.Empty, migrationTimer.GetMigrationActionTime(actionName));
    }

    [AvaloniaFact]
    public void UpdateMigrationTimes_WhenActionAlreadyExists_NoErrors()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);
        string actionName = "TestAction";

        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationActionCompleted, actionName);
        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationActionCompleted, actionName);
    }

    [AvaloniaFact]
    public void UpdateMigrationTimes_WhenActionEmpty_DoNothing()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);
        string actionName = string.Empty;

        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationActionCompleted, actionName);

        Assert.Equal(string.Empty, migrationTimer.GetMigrationActionTime(actionName));
    }

    [AvaloniaFact]
    public void Reset_ShouldClearAllTimes()
    {
        var loggerMock = new Mock<ILogger<MigrationTimer>>();
        var migrationTimer = new MigrationTimer(loggerMock.Object);

        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationStarted);
        migrationTimer.UpdateMigrationTimes(MigrationTimerEventType.MigrationActionCompleted, "TestAction");
        migrationTimer.Reset();

        Assert.Equal(string.Empty, migrationTimer.GetTotalMigrationTime);
        Assert.Equal(string.Empty, migrationTimer.GetMigrationActionTime("TestAction"));
    }
}