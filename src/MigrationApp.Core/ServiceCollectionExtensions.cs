using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Hooks.Progression;
using MigrationApp.Core.Interfaces;
using MigrationApp.Core.Services;
using Tableau.Migration;
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
            services.AddScoped<EmailDomainMapping>();
            services.AddScoped<MigrationActionProgressHook>();
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
