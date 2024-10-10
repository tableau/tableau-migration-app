// <copyright file="ServiceCollectionExtensions.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
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
        AppSettings appSettings = new AppSettings
        {
            UseSimulator = configuration.GetValue<bool>("AppSettings:UseSimulator", false),
        };
        services.AddSingleton(appSettings);
        services.AddTableauMigrationSdk(configuration.GetSection("tableau:migrationSdk"));
        services.AddScoped<ITableauMigrationService, TableauMigrationService>();
        services.AddScoped<DictionaryUserMapping>();
        services.AddScoped<EmailDomainMapping>();
        services.AddScoped<MigrationActionProgressHook>();
        services.AddScoped(typeof(BatchMigrationCompletedProgressHook<>));
        return services;
    }

    /// <summary>
    /// Constructs the configuration object needed for the migration app service.
    /// </summary>
    /// <returns>The configuration for the migration app service.</returns>
    public static IConfiguration BuildConfiguration()
    {
        var baseConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // Set a new config to make sure that the UserAgent string isn't being set through the json file
        var finalConfig = new ConfigurationBuilder()
            .AddConfiguration(baseConfig)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "tableau:migrationSdk:network:UserAgentComment", "TableauMigrationApp" }, // Fixed User Agent for the SDK
            })
            .Build();

        return finalConfig;
    }
}