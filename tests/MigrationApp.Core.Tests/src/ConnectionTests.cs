// <copyright file="ConnectionTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
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