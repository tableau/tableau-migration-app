// <copyright file="ITableauMigrationService.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Interfaces
{
    using MigrationApp.Core.Entities;

    /// <summary>
    /// Interface for an object that handles the Tableau Migration logic.
    /// </summary>
    public interface ITableauMigrationService
    {
        /// <summary>
        /// Builds a Tableau Migration SDK MigrationPlan from provided credentials.
        /// </summary>
        /// <param name="serverCreds"> Credentials for source Tableau Server. </param>
        /// <param name="cloudCreds"> Credentials for target Tableau Cloud. </param>
        /// <returns> Whether or not the Migration Plan was successfully built.</returns>
        bool BuildMigrationPlan(EndpointOptions serverCreds, EndpointOptions cloudCreds);

        /// <summary>
        /// Begin the migration from Tableau Server to Tableau Cloud.
        /// </summary>
        /// <param name="cancel"> The cancellation token to interrupt the running task. </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation status.</returns>
        Task<bool> StartMigrationTaskAsync(CancellationToken cancel);

        /// <summary>
        /// Retuerns a value as to whether a plan was properly built from existing credentials.
        /// </summary>
        /// <returns> Whether a plan was built.</returns>
        bool IsMigrationPlanBuilt();
    }
}