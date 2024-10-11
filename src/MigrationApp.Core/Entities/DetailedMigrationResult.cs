// <copyright file="DetailedMigrationResult.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Entities;

using System;
using System.Collections.Generic;
using MigrationApp.Core.Interfaces;

/// <summary>
/// Represents the detailed result of a migration run.
/// </summary>
public record struct DetailedMigrationResult(
    ITableauMigrationService.MigrationStatus status,
    IReadOnlyList<Exception> errors);
