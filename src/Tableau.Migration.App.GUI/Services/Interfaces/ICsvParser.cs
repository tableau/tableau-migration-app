// <copyright file="ICsvParser.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Services.Interfaces;

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