// <copyright file="ProgressUpdaterTests.cs" company="Salesforce, Inc.">
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
        Assert.Equal("User", progressUpdater.CurrentMigrationStateName);
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