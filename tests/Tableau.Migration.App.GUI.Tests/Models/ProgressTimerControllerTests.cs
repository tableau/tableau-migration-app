// <copyright file="ProgressTimerControllerTests.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Tests.Models;

using System;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.Models;
using Xunit;

public class ProgressTimerControllerTests
{
    [Fact]
    public void StartMigrationTimer_ShouldInvoke_OnMigrationStarted_Event()
    {
        // Arrange
        var timerController = new ProgressTimerController();
        bool eventFired = false;

        timerController.OnMigrationStarted += () => eventFired = true;

        // Act
        timerController.StartMigrationTimer();

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void CompleteMigration_ShouldInvoke_OnMigrationCompleted_Event()
    {
        // Arrange
        var timerController = new ProgressTimerController();
        bool eventFired = false;

        timerController.OnMigrationCompleted += () => eventFired = true;

        // Act
        timerController.CompleteMigration();

        // Assert
        Assert.True(eventFired);
    }

    [Fact]
    public void Should_Not_Invoke_OnMigrationStarted_If_Not_Subscribed()
    {
        // Arrange
        var timerController = new ProgressTimerController();
        bool eventFired = false;

        // Act
        timerController.StartMigrationTimer();

        // Assert
        Assert.False(eventFired);
    }

    [Fact]
    public void Should_Not_Invoke_OnMigrationCompleted_If_Not_Subscribed()
    {
        // Arrange
        var timerController = new ProgressTimerController();
        bool eventFired = false;

        // Act
        timerController.CompleteMigration();

        // Assert
        Assert.False(eventFired);
    }
}