// <copyright file="CsvHelperParser.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Services.Implementations;
using CsvHelper;
using MigrationApp.GUI.Models;
using MigrationApp.GUI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

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