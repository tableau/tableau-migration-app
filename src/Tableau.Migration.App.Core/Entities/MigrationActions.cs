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
    /// <summary>
    /// Gets the list of actions available from the Tableau Migration SDK and the order in which they are migrated.
    /// </summary>
    public static List<string> Actions
    {
        get
        {
            var result = ServerToCloudMigrationPipeline
                .ContentTypes
                .Select(
                    contentType => GetActionTypeName(contentType.ContentType)).ToList();
            return result;
        }
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