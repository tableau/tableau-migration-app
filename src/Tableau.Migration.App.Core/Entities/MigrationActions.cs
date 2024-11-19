// <copyright file="MigrationActions.cs" company="Salesforce, Inc.">
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
using System.Linq;
using System.Text.RegularExpressions;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Pipelines;

/// <summary>
/// Details of Tableau Migration SDK Migration Actions.
/// </summary>
public class MigrationActions
{
    private static List<string> actionsCache = new List<string>();

    /// <summary>
    /// Gets the list of actions available from the Tableau Migration SDK and the order in which they are migrated.
    /// </summary>
    public static List<string> Actions
    {
        get
        {
            if (actionsCache.Count > 0)
            {
                return actionsCache;
            }

            actionsCache = ServerToCloudMigrationPipeline
                .ContentTypes
                .Select(
                    contentType => GetActionTypeName(contentType.ContentType)).ToList();
            actionsCache.Insert(0, "Setup");

            return actionsCache;
        }
    }

    /// <summary>
    /// Get the index of a migration action.
    /// </summary>
    /// <param name="action">The migration action name.</param>
    /// <returns>The migration index for the provided action.</returns>
    public static int GetActionIndex(string action)
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            if (action == Actions[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the action type names from the SDK.
    /// </summary>
    /// <param name="contentType">The type to get the name for.</param>
    /// <returns>The human readable type name.</returns>
    public static string GetActionTypeName(Type contentType)
    {
        string typeName = MigrationPipelineContentType.GetConfigKeyForType(contentType);

        // Add a space before every Capitalized letter that's not the first.
        typeName = Regex.Replace(typeName, "(?<!^)([A-Z])", " $1");
        return typeName;
    }
}