// <copyright file="TableauEnv.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Models;
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