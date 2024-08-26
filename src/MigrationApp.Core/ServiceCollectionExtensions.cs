using Microsoft.Extensions.DependencyInjection;
using MigrationApp.Core.Interfaces;
using MigrationApp.Core.Services;
using Tableau.Migration;

namespace MigrationApp.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMigrationAppCore(this IServiceCollection services)
        {
            services.AddTableauMigrationSdk();
            services.AddScoped<ITableauMigrationService, TableauMigrationService>();
            return services;
        }
    }
}
