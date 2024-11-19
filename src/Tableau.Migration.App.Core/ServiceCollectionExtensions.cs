// <copyright file="ServiceCollectionExtensions.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Hooks.Progression;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.Core.Services;
using Tableau.Migration.Engine.Manifest;

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
        services.AddSingleton<MigrationManifestSerializer>();
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