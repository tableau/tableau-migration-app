// <copyright file="ProgressUpdaterTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Tests.Models;

using MigrationApp.GUI.Models;

public class ProgressUpdaterTests
{
    [Fact]
    public void ProgressUpdater_Should_Have_Initial_State()
    {
        var progressUpdater = new ProgressUpdater();

        Assert.Equal(-1, progressUpdater.CurrentMigrationStateIndex);
        Assert.Equal(string.Empty, progressUpdater.CurrentMigrationStateName);
    }

    [Fact]
    public void ProgressUpdater_Should_Update_State_On_Update_Call()
    {
        /**
         * ProgressUpdater should update both CurrentMigraitonStateIndex and CurrentMigrationStateName on update call.
         * CurrentMigrationStateIndex should not be able to go the amount of states
         */
        var progressUpdater = new ProgressUpdater();

        progressUpdater.Update();

        Assert.Equal(0, progressUpdater.CurrentMigrationStateIndex);
        Assert.Equal("Users", progressUpdater.CurrentMigrationStateName);
        progressUpdater.CurrentMigrationStateIndex = ProgressUpdater.NumMigrationStates + 1;
        Assert.Equal(0, progressUpdater.CurrentMigrationStateIndex); // Don't update if state index is out of bounds

        // At max value, the name should be empty again
        progressUpdater.CurrentMigrationStateIndex = ProgressUpdater.NumMigrationStates;
        Assert.Equal(string.Empty, progressUpdater.CurrentMigrationStateName);
    }

    [Fact]
    public void ProgressUpdater_Should_Fire_OnProgressChanged_Event_When_State_Changes()
    {
        var progressUpdater = new ProgressUpdater();
        bool eventFired = false;

        progressUpdater.OnProgressChanged += (sender, args) => eventFired = true;

        progressUpdater.Update();

        Assert.True(eventFired);
    }
}