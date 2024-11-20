// <copyright file="DetailedMigrationResults.cs" company="Salesforce, Inc.">
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

namespace DetailedMigrationResultsTest;

using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.Core.Services;
using Xunit;

public class DetailedMigrationResultsTests
{
    [Fact]
    public void Constructor_ValidParameters()
    {
        var expectedStatus = ITableauMigrationService.MigrationStatus.SUCCESS;
        var expectedErrors = new List<Exception> { new Exception("Test error") };

        var result = new DetailedMigrationResult(expectedStatus, expectedErrors);

        Assert.Equal(expectedStatus, result.status);
        Assert.Equal(expectedErrors, result.errors);
    }

    [Fact]
    public void RecordEquality_ShouldBeTrue_WhenPropertiesAreEqual()
    {
        var status = ITableauMigrationService.MigrationStatus.SUCCESS;
        var errors = new List<Exception> { new Exception("Test error") };

        var result1 = new DetailedMigrationResult(status, errors);
        var result2 = new DetailedMigrationResult(status, errors);

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void RecordEquality_ShouldBeFalse_WhenPropertiesAreDifferent()
    {
        var status1 = ITableauMigrationService.MigrationStatus.SUCCESS;
        var errors1 = new List<Exception> { new Exception("Test error 1") };

        var status2 = ITableauMigrationService.MigrationStatus.FAILURE;
        var errors2 = new List<Exception> { new Exception("Test error 2") };

        var result1 = new DetailedMigrationResult(status1, errors1);
        var result2 = new DetailedMigrationResult(status2, errors2);

        Assert.NotEqual(result1, result2);
    }
}