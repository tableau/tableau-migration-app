// <copyright file="ConnectionTests.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.MigrationApp.Core.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;

public class ConnectionTests
{
    [Fact(Skip = "Disabled until TMAPP-53 resolved.")]
    public async void ConnectionTest()
    {
        // Default builder setup
        using var host = Host.CreateDefaultBuilder()
                      .ConfigureServices((ctx, services) =>
                      {
                          services.AddTableauMigrationSdk();
                      })
                      .Build();
        var logger = host.Services.GetRequiredService<ILogger<ConnectionTests>>();
        var config = host.Services.GetRequiredService<IConfiguration>();
        var planBuilder = host.Services.GetRequiredService<IMigrationPlanBuilder>();

        // Load credentials from env vars
        planBuilder
            .FromSourceTableauServer(
                new Uri(config["TABLEAU_SERVER_URL"] ?? string.Empty),
                config["TABLEAU_SERVER_SITE"] ?? string.Empty,
                config["TABLEAU_SERVER_TOKEN_NAME"] ?? string.Empty,
                config["TABLEAU_SERVER_TOKEN"] ?? string.Empty,
                true) // use simulator to avoid failing the tests on PRs
            .ToDestinationTableauCloud(
                new Uri(config["TABLEAU_CLOUD_URL"] ?? string.Empty),
                config["TABLEAU_CLOUD_SITE"] ?? string.Empty,
                config["TABLEAU_CLOUD_TOKEN_NAME"] ?? string.Empty,
                config["TABLEAU_CLOUD_TOKEN"] ?? string.Empty,
                true)
            .ForServerToCloud()
            .WithTableauIdAuthenticationType();

        planBuilder.Validate();

        var plan = planBuilder.Build();
        var migrator = host.Services.GetRequiredService<IMigrator>();

        // Not calling from a registered service, so don't have a cancellation token to provide.
        // As is, will hang indefinitely if connection fails
        var results = await migrator.ExecuteAsync(plan, null, CancellationToken.None);

        Assert.Equal("Completed", results.Status.ToString());
    }
}