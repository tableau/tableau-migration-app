// <copyright file="MigrationActions.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Entities;
using Tableau.Migration;
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