// <copyright file="AppSettings.cs" company="Salesforce, Inc.">
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

namespace AppSettingsTests;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using Tableau.Migration;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Hooks.Progression;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.Core.Services;
using Tableau.Migration.Engine.Manifest;
using Xunit;

public class AppSettingsTest
{
    [Fact]
    public async void AppSettings_UseSimulator_True()
    {
        var services = new ServiceCollection();
        var appSettings = new AppSettings();
        appSettings.UseSimulator = true;
        services.AddSingleton(appSettings);
        services.AddSingleton<MigrationManifestSerializer>();
        services.AddTableauMigrationSdk();
        services.AddScoped<ITableauMigrationService, TableauMigrationService>();
        services.AddScoped<DictionaryUserMapping>();
        services.AddScoped<EmailDomainMapping>();
        services.AddScoped<MigrationActionProgressHook>();
        services.AddScoped(typeof(BatchMigrationCompletedProgressHook<>));

        var provider = services.BuildServiceProvider();
        var tableauMigrationService = provider.GetRequiredService<ITableauMigrationService>();

        EndpointOptions serverEndpoint = new EndpointOptions("http://url/", "site", "token name", "token");
        EndpointOptions cloudEndpoint = new EndpointOptions("http://url/", "site", "token name", "token");

        tableauMigrationService.BuildMigrationPlan(serverEndpoint, cloudEndpoint);
        var result = await tableauMigrationService.StartMigrationTaskAsync(CancellationToken.None);

        // Migrator service should successfully run with simulator even if endpoint data is incorrect
        Assert.Equal(ITableauMigrationService.MigrationStatus.SUCCESS, result.status);
    }

    [Fact(Skip = "Skipped since SDK Retries will make this test take several minutes to complete.")]
    public async void AppSettings_UseSimulator_False()
    {
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        var appSettings = new AppSettings();
        appSettings.UseSimulator = false;
        services.AddSingleton(appSettings);
        services.AddSingleton<MigrationManifestSerializer>();
        services.AddTableauMigrationSdk();
        services.AddScoped<ITableauMigrationService, TableauMigrationService>();
        services.AddScoped<DictionaryUserMapping>();
        services.AddScoped<EmailDomainMapping>();
        services.AddScoped<MigrationActionProgressHook>();
        services.AddScoped(typeof(BatchMigrationCompletedProgressHook<>));

        var provider = services.BuildServiceProvider();
        var tableauMigrationService = provider.GetRequiredService<ITableauMigrationService>();

        EndpointOptions serverEndpoint = new EndpointOptions("http://url/", "site", "token name", "token");
        EndpointOptions cloudEndpoint = new EndpointOptions("http://url/", "site", "token name", "token");

        tableauMigrationService.BuildMigrationPlan(serverEndpoint, cloudEndpoint);
        var result = await tableauMigrationService.StartMigrationTaskAsync(CancellationToken.None);

        // Migrator service should successfully run with simulator even if endpoint data is incorrect
        Assert.Equal(ITableauMigrationService.MigrationStatus.FAILURE, result.status);
    }
}