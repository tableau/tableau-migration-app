// <copyright file="DetailedMigrationResult.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Entities;

using System;
using System.Collections.Generic;
using Tableau.Migration.App.Core.Interfaces;

/// <summary>
/// Represents the detailed result of a migration run.
/// </summary>
public record struct DetailedMigrationResult(
    ITableauMigrationService.MigrationStatus status,
    IReadOnlyList<Exception> errors);

/// <summary>
/// Builder to build a <see cref="DetailedMigrationResult" />.
/// </summary>
public static class DetailedMigrationResultBuilder
{
    /// <summary>
    /// Build a Detailed Migration Result.
    /// </summary>
    /// <param name="status">The Tableau Migration SDK Status.</param>
    /// <param name="errors">The errors from migration.</param>
    /// <returns>The <see cref="DetailedMigrationResult" />.</returns>
    public static DetailedMigrationResult Build(
        MigrationCompletionStatus status, IReadOnlyList<Exception> errors)
    {
        ITableauMigrationService.MigrationStatus newStatus;
        switch (status)
        {
            case MigrationCompletionStatus.Completed:
                newStatus = ITableauMigrationService.MigrationStatus.SUCCESS;
                break;
            case MigrationCompletionStatus.Canceled:
                newStatus = ITableauMigrationService.MigrationStatus.CANCELED;
                break;
            default: // ITableauMigrationService.MigrationStatus.FAILURE
                newStatus = ITableauMigrationService.MigrationStatus.FAILURE;
                break;
        }

        return new DetailedMigrationResult(newStatus, errors);
    }
}