using MigrationApp.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.MigrationApp.Core.Tests;

public class ConfigLoadTests
{
    [Fact]
    public void ConfigLoadTest()
    {
        IConfiguration configuration = ServiceCollectionExtensions.BuildConfiguration();
        using var host = Host.CreateDefaultBuilder()
                      .ConfigureServices((ctx, services) =>
                      {
                          services.AddTableauMigrationSdk();
                          services.AddMigrationAppCore(configuration);
                      })
                      .Build();
        var appSettings = host.Services.GetRequiredService<AppSettings>();
        return;
    }
}
