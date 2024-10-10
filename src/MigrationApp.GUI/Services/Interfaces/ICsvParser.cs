// <copyright file="ICsvParser.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Defines a contract for parsing CSV files into a dictionary mapping.
/// </summary>
public interface ICsvParser
{
    /// <summary>
    /// Asynchronously parses a CSV file and returns a dictionary mapping column 1 to column 2.
    /// </summary>
    /// <param name="filePath">The path to the CSV file to parse.</param>
    /// <returns>A task representing the asynchronous operation, with a dictionary result.</returns>
    Task<Dictionary<string, string>> ParseAsync(string filePath);
}