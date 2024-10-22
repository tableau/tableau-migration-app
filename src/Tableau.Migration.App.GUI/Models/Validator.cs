// <copyright file="Validator.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Models;
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