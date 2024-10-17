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

namespace MigrationApp.Core.Entities;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;

/// <summary>
/// Details of Tableau Migration SDK Migration Actions.
/// </summary>
public class MigrationActions
{
    /// <summary>
    /// Dictionary mapping IContentReference types to their names.
    /// </summary>
    public static readonly Dictionary<Type, string> ActionNameMapping =
        new Dictionary<Type, string>
        {
            { typeof(IUser), "Users" },
            { typeof(IGroup), "Groups" },
            { typeof(IProject), "Projects" },
            { typeof(IDataSource), "Data Sources" },
            { typeof(IWorkbook), "Workbooks" },
            { typeof(IServerExtractRefreshTask), "Extract Refresh Tasks" },
            { typeof(ICustomView), "Custom Views" },
        };

    /// <summary>
    /// List of actions available from the Tableau Migration SDK and the order in which they are migrated.
    /// </summary>
    public static readonly string[] Actions = { "Users", "Groups", "Projects", "DataSources", "Workbooks", "ServerExtractRefreshTasks", "CustomViews" };
}