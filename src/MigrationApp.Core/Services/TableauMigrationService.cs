// <copyright file="TableauMigrationService.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.Core.Services;
using Microsoft.Extensions.Logging;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Hooks.Progression;
using MigrationApp.Core.Interfaces;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;

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
    private readonly IProgressMessagePublisher? publisher;
    private IMigrationPlan? plan;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableauMigrationService" /> class.
    /// </summary>
    /// <param name="planBuilder">The Tableau Migration SDK PlanBuilder object.</param>
    /// <param name="migrator">The Tableau Migration SDK Migrator object.</param>
    /// <param name="logger">The logger to be used.</param>
    /// <param name="appSettings">The application settings to apply to the application.</param>
    /// <param name="progressUpdater">The object to handle the visual migration progress indicator.</param>
    /// <param name="publisher">The message publisher to broadcast progress updates.</param>
    public TableauMigrationService(
        IMigrationPlanBuilder planBuilder,
        IMigrator migrator,
        ILogger<TableauMigrationService> logger,
        AppSettings appSettings,
        IProgressUpdater? progressUpdater = null,
        IProgressMessagePublisher? publisher = null)
    {
        this.appSettings = appSettings;
        this.planBuilder = planBuilder;
        this.migrator = migrator;
        this.logger = logger;
        this.progressUpdater = progressUpdater;
        this.publisher = publisher;
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
            .WithTableauCloudUsernames<EmailDomainMapping>()
            .WithTableauCloudUsernames<DictionaryUserMapping>(); // Prioritize user defined mappings over set emails

        var validationResult = this.planBuilder.Validate();

        if (!validationResult.Success)
        {
            this.logger.LogError("Migration plan validation failed. {Errors}", validationResult.Errors);
            return false;
        }

        // Progression Hooks
        this.planBuilder.Hooks.Add<MigrationActionProgressHook>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<IUser>>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<IGroup>>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<IProject>>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<IDataSource>>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<IWorkbook>>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<ICloudExtractRefreshTask>>();
        this.planBuilder.Hooks.Add<BatchMigrationCompletedProgressHook<ICustomView>>();

        this.plan = this.planBuilder.Build();

        return true;
    }

    /// <inheritdoc/>
    public async Task<DetailedMigrationResult> StartMigrationTaskAsync(CancellationToken cancel)
    {
        if (this.plan == null)
        {
            this.logger.LogError("Migration plan is not built.");
            return new DetailedMigrationResult(ITableauMigrationService.MigrationStatus.FAILURE, new List<Exception>());
        }

        var result = await this.migrator.ExecuteAsync(this.plan, cancel);
        List<string> messageList = new ();
        var manifest = result.Manifest;
        IReadOnlyList<Exception> errors = manifest.Errors;
        var statusIcon = IProgressMessagePublisher.GetStatusIcon(IProgressMessagePublisher.MessageStatus.Error);
        foreach (var error in errors)
        {
            try
            {
                ErrorMessage parsedError = new ErrorMessage(error.Message);
                messageList.Add($"\t {statusIcon} {parsedError.Detail}");
                messageList.Add($"\t\t {parsedError.Summary}: {parsedError.URL}");
            }
            catch (Exception)
            {
                messageList.Add($"\t {statusIcon} Could not parse error message: \n{error.Message}");
            }
        }

        string resultErrorMessage = string.Join("\n", messageList);

        if (result.Status == MigrationCompletionStatus.Completed)
        {
            this.logger.LogInformation("Migration completed.");
            this.publisher?.PublishProgressMessage("Migration completed", resultErrorMessage);
            return new DetailedMigrationResult(ITableauMigrationService.MigrationStatus.SUCCESS, errors);
        }
        else if (result.Status == MigrationCompletionStatus.Canceled)
        {
            this.logger.LogInformation("Migration cancelled.");
            this.publisher?.PublishProgressMessage("Migration cancelled", resultErrorMessage);
            return new DetailedMigrationResult(ITableauMigrationService.MigrationStatus.CANCELLED, errors);
        }
        else
        {
            this.logger.LogError("Migration failed with status: {Status}", result.Status);
            this.publisher?.PublishProgressMessage("Migration failed", resultErrorMessage);
            return new DetailedMigrationResult(ITableauMigrationService.MigrationStatus.FAILURE, errors);
        }
    }

    /// <inheritdoc/>
    public bool IsMigrationPlanBuilt()
    {
        return this.plan != null;
    }
}