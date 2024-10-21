// <copyright file="MigrationActions.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationActions;
using MigrationApp.Core.Entities;
using System;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Xunit;

public class MigrationActionsTest
{
    [Fact]
    public void GetActionTypeNames_MakesNamesReadable()
    {
        Assert.Equal("User", MigrationActions.GetActionTypeName(typeof(IUser)));
        Assert.Equal("Server Extract Refresh Task", MigrationActions.GetActionTypeName(typeof(IServerExtractRefreshTask)));
    }
}