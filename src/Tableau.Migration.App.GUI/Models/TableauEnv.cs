// <copyright file="TableauEnv.cs" company="Salesforce, Inc.">
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
using System.Collections.Generic;

/// <summary>
/// The desired tableau environment of this control.
/// </summary>
public enum TableauEnv
{
    /// <summary>
    /// Tableau Server environment.
    /// </summary>
    TableauServer,

    /// <summary>
    /// Tableau Cloud environment.
    /// </summary>
    TableauCloud,
}

/// <summary>
/// Utility functionality for identifying Tableau Environments.
/// </summary>
public class TableauEnvMapper
{
    private static readonly IReadOnlyDictionary<TableauEnv, string> TableauEnvMap =
        new Dictionary<TableauEnv, string>
        {
            { TableauEnv.TableauServer, "Server" },
            { TableauEnv.TableauCloud, "Cloud" },
        };

    /// <summary>
    /// Returns the string representation of the given Tableau Environment.
    /// </summary>
    /// <param name="env">The Tableau environment.</param>
    /// <returns>The string representation of the Tableau environment.</returns>
    public static string ToString(TableauEnv env)
    {
        return TableauEnvMapper.TableauEnvMap[env];
    }
}