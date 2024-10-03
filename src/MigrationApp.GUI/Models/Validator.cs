// <copyright file="Validator.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Models;
using System.Text.RegularExpressions;

/// <summary>
/// Collection of validation functions of Migration App data.
/// </summary>
public class Validator
{
    private const string DomainPattern = @"^((?!-)[A-Za-z0-9-]{1,63}(?<!-)\.)+[A-Za-z]{2,20}$";

    /// <summary>
    /// Determines whether or not the provided domain is a valid one to be used for Tableau Cloud.
    /// </summary>
    /// <param name="domain">The domain to check.</param>
    /// <returns>Whether or not the domain is valid.</returns>
    public static bool IsDomainNameValid(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            return false;
        }

        return Regex.IsMatch(domain, DomainPattern);
    }
}