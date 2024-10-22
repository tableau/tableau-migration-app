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

namespace Tableau.Migration.App.Core.Services;
using Microsoft.Extensions.Logging;
using Tableau.Migration;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Hooks.Progression;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Manifest;

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
    private readonly MigrationManifestSerializer manifestSerializer;
    private IMigrationPlan? plan;
    private IMigrationManifest? manifest;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableauMigrationService" /> class.
    /// </summary>
    /// <param name="planBuilder">The Tableau Migration SDK PlanBuilder object.</param>
    /// <param name="migrator">The Tableau Migration SDK Migrator object.</param>
    /// <param name="logger">The logger to be used.</param>
    /// <param name="appSettings">The application settings to apply to the application.</param>
    /// <param name="progressUpdater">The object to handle the visual migration progress indicator.</param>
    /// <param name="publisher">The message publisher to broadcast progress updates.</param>
    /// <param name="manifestSerializer">Serializaer class to save and load manifest file.</param>
    public TableauMigrationService(
        IMigrationPlanBuilder planBuilder,
        IMigrator migrator,
        ILogger<TableauMigrationService> logger,
        AppSettings appSettings,
        MigrationManifestSerializer manifestSerializer,
        IProgressUpdater? progressUpdater = null,
        IProgressMessagePublisher? publisher = null)
    {
        this.appSettings = appSettings;
        this.planBuilder = planBuilder;
        this.migrator = migrator;
        this.logger = logger;
        this.progressUpdater = progressUpdater;
        this.publisher = publisher;
        this.manifestSerializer = manifestSerializer;
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

        return await this.ExecuteMigrationAsync(this.plan, null, cancel);
    }

    /// <inheritdoc/>
    public async Task<DetailedMigrationResult> ResumeMigrationTaskAsync(string manifestFilepath, CancellationToken cancel)
    {
        if (this.plan == null)
        {
            this.logger.LogError("Migration plan is not built.");
            return new DetailedMigrationResult(ITableauMigrationService.MigrationStatus.FAILURE, new List<Exception>());
        }

        if (!(await this.LoadManifestAsync(manifestFilepath, cancel)))
        {
            this.logger.LogError("Migration manifest is null. Unable to resume the migration");
            return new DetailedMigrationResult(ITableauMigrationService.MigrationStatus.FAILURE, new List<Exception>());
        }

        return await this.ExecuteMigrationAsync(this.plan, this.manifest, cancel);
    }

    /// <inheritdoc/>
    public bool IsMigrationPlanBuilt()
    {
        return this.plan != null;
    }

    /// <inheritdoc/>
    public async Task<bool> SaveManifestAsync(string manifestFilepath)
    {
        if (this.manifest == null)
        {
            this.logger?.LogWarning("Manifest is null, unable to save.");
            return false;
        }

        if (string.IsNullOrEmpty(manifestFilepath))
        {
            this.logger?.LogWarning("Invalid manifest file path.");
            return false;
        }

        try
        {
            await this.manifestSerializer.SaveAsync(this.manifest, manifestFilepath);
            this.logger?.LogInformation($"Manifest saved successfully at {manifestFilepath}.");
            return true;
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, $"Failed to save manifest to {manifestFilepath}: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> LoadManifestAsync(string manifestFilepath, CancellationToken cancel)
    {
        if (string.IsNullOrEmpty(manifestFilepath))
        {
            this.logger?.LogWarning("Invalid manifest file path.");
            return false;
        }

        try
        {
            this.manifest = await this.manifestSerializer.LoadAsync(manifestFilepath, cancel);

            if (this.manifest != null)
            {
                this.logger?.LogInformation($"Manifest loaded successfully from {manifestFilepath}.");
                return true;
            }
            else
            {
                this.logger?.LogWarning($"Manifest is null after loading from {manifestFilepath}.");
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            this.logger?.LogWarning("Manifest loading was canceled.");
            return false;
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, $"Failed to load manifest from {manifestFilepath}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Executes the migration and handles the common result processing.
    /// </summary>
    /// <param name="plan">The migration plan.</param>
    /// <param name="manifest">The optional migration manifest, used in resume operations.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <returns>The detailed migration result.</returns>
    private async Task<DetailedMigrationResult> ExecuteMigrationAsync(IMigrationPlan plan, IMigrationManifest? manifest, CancellationToken cancel)
    {
        MigrationResult result;

        result = await this.migrator.ExecuteAsync(plan, manifest, cancel);

        List<string> messageList = new ();
        this.manifest = result.Manifest;
        IReadOnlyList<Exception> errors = this.manifest.Errors;
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
}