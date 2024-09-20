// <copyright file="ServiceCollectionExtensions.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Hooks.Progression;
using MigrationApp.Core.Interfaces;
using MigrationApp.Core.Services;
using Tableau.Migration;

/// <summary>
/// Dependency Injection utility extensions to setup migration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Sets up and injects the Migration App to the provided service collection.
    /// </summary>
    /// <param name="services">The service collection to inject into.</param>
    /// <param name="configuration">The configurations for the app to use.</param>
    /// <returns>The service with the migration app injected.</returns>
    public static IServiceCollection AddMigrationAppCore(this IServiceCollection services, IConfiguration configuration)
    {
        AppSettings appSettings = new ();
        configuration.GetSection("AppSettings").Bind(appSettings);
        services.AddSingleton(appSettings);

        services.AddTableauMigrationSdk();
        services.AddScoped<ITableauMigrationService, TableauMigrationService>();
        services.AddScoped<EmailDomainMapping>();
        services.AddScoped<MigrationActionProgressHook>();
        return services;
    }

    /// <summary>
    /// Constructs the configuration object needed for the migration app service.
    /// </summary>
    /// <returns>The configuration for the migration app service.</returns>
    public static IConfiguration BuildConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AppSettings:useSimulator", "false" }, // Default value
            })
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        return configurationBuilder.Build();
    }
}