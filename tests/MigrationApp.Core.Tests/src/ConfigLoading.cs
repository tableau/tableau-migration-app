// <copyright file="ConfigLoading.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tableau.Migration.MigrationApp.Core.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationApp.Core;

public class ConfigLoadTests
{
    [Fact(Skip = "Disabled until TMAPP-53 resolved.")]
    public void ConfigLoadTest()
    {
        // IConfiguration configuration = ServiceCollectionExtensions.BuildConfiguration();
        // using var host = Host.CreateDefaultBuilder()
        //               .ConfigureServices((ctx, services) =>
        //               {
        //                   services.AddTableauMigrationSdk();
        //                   services.AddMigrationAppCore(configuration);
        //               })
        //               .Build();
        // var appSettings = host.Services.GetRequiredService<AppSettings>();
        return;
    }
}