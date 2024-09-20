// <copyright file="TableauMigrationService.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Services;
using Microsoft.Extensions.Logging;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Hooks.Progression;
using MigrationApp.Core.Interfaces;
using Tableau.Migration;

/// <summary>
/// Service to handle Migrations from Tableau Server to Tableau Cloud.
/// </summary>
public class TableauMigrationService : ITableauMigrationService
{
    private readonly AppSettings appSettings;
    private readonly IMigrationPlanBuilder planBuilder;
    private readonly IMigrator migrator;
    private readonly ILogger<TableauMigrationService> logger;
    private readonly IProgressUpdater? progressUpdater;
    private IMigrationPlan? plan;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableauMigrationService" /> class.
    /// </summary>
    /// <param name="planBuilder">The Tableau Migration SDK PlanBuilder object.</param>
    /// <param name="migrator">The Tableau Migration SDK Migrator object.</param>
    /// <param name="logger">The logger to be used.</param>
    /// <param name="appSettings">The application settings to apply to the application.</param>
    /// <param name="progressUpdater">The object to handle the visual migration progress indicator.</param>
    public TableauMigrationService(
        IMigrationPlanBuilder planBuilder,
        IMigrator migrator,
        ILogger<TableauMigrationService> logger,
        AppSettings appSettings,
        IProgressUpdater? progressUpdater = null)
    {
        this.appSettings = appSettings;
        this.planBuilder = planBuilder;
        this.migrator = migrator;
        this.logger = logger;
        this.progressUpdater = progressUpdater;
    }

    /// <inheritdoc/>
    public bool BuildMigrationPlan(EndpointOptions serverEndpoints, EndpointOptions cloudEndpoints)
    {
        this.progressUpdater?.Update();
        this.planBuilder
            .FromSourceTableauServer(
                serverEndpoints.Url,
                serverEndpoints.SiteContentUrl,
                serverEndpoints.AccessTokenName,
                serverEndpoints.AccessToken,
                this.appSettings.UseSimulator)
            .ToDestinationTableauCloud(
                cloudEndpoints.Url,
                cloudEndpoints.SiteContentUrl,
                cloudEndpoints.AccessTokenName,
                cloudEndpoints.AccessToken,
                this.appSettings.UseSimulator)
            .ForServerToCloud()
            .WithTableauIdAuthenticationType()
            .WithTableauCloudUsernames<EmailDomainMapping>();

        var validationResult = this.planBuilder.Validate();

        if (!validationResult.Success)
        {
            this.logger.LogError("Migration plan validation failed. {Errors}", validationResult.Errors);
            return false;
        }

        this.planBuilder.Hooks.Add<MigrationActionProgressHook>();
        this.plan = this.planBuilder.Build();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> StartMigrationTaskAsync(CancellationToken cancel)
    {
        if (this.plan == null)
        {
            this.logger.LogError("Migration plan is not built.");
            return false;
        }

        var result = await this.migrator.ExecuteAsync(this.plan, cancel);

        if (result.Status == MigrationCompletionStatus.Completed)
        {
            this.logger.LogInformation("Migration succeeded.");
            return true;
        }
        else if (result.Status == MigrationCompletionStatus.Canceled)
        {
            this.logger.LogInformation("Migration cancelled.");
            return true;
        }
        else
        {
            this.logger.LogError("Migration failed with status: {Status}", result.Status);
            return false;
        }
    }

    /// <inheritdoc/>
    public bool IsMigrationPlanBuilt()
    {
        return this.plan != null;
    }
}