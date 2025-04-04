// <copyright file="CsvHelperParser.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Services.Implementations;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.Services.Interfaces;

/// <summary>
/// Implements the ICsvParser interface using the CsvHelper library.
/// </summary>
public class CsvHelperParser : ICsvParser
{
    /// <summary>
    /// Asynchronously parses a CSV file using CsvHelper and returns a dictionary mapping column 1 to column 2.
    /// </summary>
    /// <param name="filePath">The path to the CSV file to parse.</param>
    /// <returns>A task representing the asynchronous operation, with a dictionary result.</returns>
    /// <exception cref="InvalidDataException">Thrown when a row does not have exactly two columns.</exception>
    public async Task<Dictionary<string, string>> ParseAsync(string filePath)
    {
        var map = new Dictionary<string, string>();

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            while (await csv.ReadAsync())
            {
                var serverUsername = csv.GetField(0)?.Trim();
                var cloudUsername = csv.GetField(1)?.Trim();

                if (serverUsername == null || cloudUsername == null
                    || serverUsername == string.Empty || cloudUsername == string.Empty
                    || csv.Parser.Count != 2)
                {
                    throw new InvalidDataException($"Invalid number of columns at row {csv.Context.Parser?.Row}");
                }

                try
                {
                    MailAddress emailValidation = new MailAddress(cloudUsername);

                    if (!Validator.IsDomainNameValid(emailValidation.Host))
                    {
                        System.Console.WriteLine(emailValidation.Host);
                        throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    throw new InvalidDataException($"Cloud username is not in proper email format at row {csv.Context.Parser?.Row}");
                }

                if (!map.ContainsKey(serverUsername))
                {
                    map.Add(serverUsername, cloudUsername);
                }
                else
                {
                    map[serverUsername] = cloudUsername; // Overwrite existing value
                }
            }
        }

        return map;
    }
}