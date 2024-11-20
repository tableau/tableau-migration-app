// <copyright file="MigrationTimerEvent.cs" company="Salesforce, Inc.">
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

namespace MigrationTimerEventTests;
using System;
using System.Collections.Generic;
using Tableau.Migration.App.Core.Entities;
using Xunit;

public class MigrationTimerEventTests
{
    [Fact]
    public void MigrationTimerEvent_Constructor_SetsEventTypeCorrectly()
    {
        var eventType = MigrationTimerEventType.MigrationStarted;
        var migrationTimerEvent = new MigrationTimerEvent(eventType);
        Assert.Equal(eventType, migrationTimerEvent.EventType);
    }

    [Fact]
    public void MigrationTimerEvent_Constructor_InitializesActionStartTimes()
    {
        var migrationTimerEvent = new MigrationTimerEvent(MigrationTimerEventType.MigrationStarted);
        Assert.NotNull(migrationTimerEvent);
        var actionStartTimes =
            migrationTimerEvent
            .GetType()
            .GetField(
                "actionStartTimes",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(migrationTimerEvent);
        Assert.NotNull(actionStartTimes);
        Assert.IsType<Dictionary<string, DateTime>>(actionStartTimes);
    }

    [Fact]
    public void MigrationTimerEvent_Constructor_InitializesActionStopTimes()
    {
        var migrationTimerEvent = new MigrationTimerEvent(MigrationTimerEventType.MigrationStarted);
        Assert.NotNull(migrationTimerEvent);
        Assert.IsType<Dictionary<string, DateTime>>(
            migrationTimerEvent
            .GetType()
            .GetField(
                "actionStopTimes",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(migrationTimerEvent));
    }

    [Fact]
    public void MigrationTimerEvent_Constructor_InitializesMigrationStartTime()
    {
        var migrationTimerEvent = new MigrationTimerEvent(MigrationTimerEventType.MigrationStarted);
        var migrationStartTime = migrationTimerEvent
            .GetType()
            .GetField(
                "migrationStartTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
            .GetValue(migrationTimerEvent);
        Assert.IsType<DateTime>(migrationStartTime);
    }
}