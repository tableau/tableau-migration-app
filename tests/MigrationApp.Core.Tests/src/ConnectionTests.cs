using Tableau.Migration.MigrationApp.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tableau.Migration.MigrationApp.Core.Tests;


using Microsoft.Extensions.Logging;
using System.Threading;


public class ConnectionTests
{
    [Fact]
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
            .FromSourceTableauServer(new Uri(config["TABLEAU_SERVER_URL"] ?? ""),
                                     config["TABLEAU_SERVER_SITE"] ?? "",
                                     config["TABLEAU_SERVER_TOKEN_NAME"] ?? "",
                                     config["TABLEAU_SERVER_TOKEN"] ?? "",
                                     true) // use simulator to avoid failing the tests on PRs
            .ToDestinationTableauCloud(new Uri(config["TABLEAU_CLOUD_URL"] ?? ""),
                                       config["TABLEAU_CLOUD_SITE"] ?? "",
                                       config["TABLEAU_CLOUD_TOKEN_NAME"] ?? "",
                                       config["TABLEAU_CLOUD_TOKEN"] ?? "",
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
