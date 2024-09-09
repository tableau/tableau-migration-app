using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigrationApp.Core.Interfaces;
using MigrationApp.Core.Services;
using Tableau.Migration;
using MigrationApp.Core.Entities;
namespace MigrationApp.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMigrationAppCore(this IServiceCollection services, IConfiguration configuration)
        {
            AppSettings appSettings = new();
            configuration.GetSection("AppSettings").Bind(appSettings);
            services.AddSingleton(appSettings);

            services.AddTableauMigrationSdk();
            services.AddScoped<ITableauMigrationService, TableauMigrationService>();
            return services;
        }

        public static IConfiguration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "AppSettings:useSimulator", "false" } // Default value
                })
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return configurationBuilder.Build();
        }

    }
}
