// <copyright file="ITableauMigrationService.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.Core.Interfaces
{
    using MigrationApp.Core.Entities;

    /// <summary>
    /// Interface for an object that handles the Tableau Migration logic.
    /// </summary>
    public interface ITableauMigrationService
    {
        /// <summary>
        /// Enum representing the status of the current migration run.
        /// </summary>
        public enum MigrationStatus
        {
            /// <summary>
            /// The Migration completed successfully.
            /// </summary>
            SUCCESS,

            /// <summary>
            /// The Migration failed.
            /// </summary>
            FAILURE,

            /// <summary>
            /// The migration was stopped while running.
            /// </summary>
            CANCELLED,

            /// <summary>
            /// The migration was resumed after previously being stopped.
            /// </summary>
            RESUMED,
        }

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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation <see cref="DetailedMigrationResult"/>.</returns>
        Task<DetailedMigrationResult> StartMigrationTaskAsync(CancellationToken cancel);

        /// <summary>
        /// Retuerns a value as to whether a plan was properly built from existing credentials.
        /// </summary>
        /// <returns> Whether a plan was built.</returns>
        bool IsMigrationPlanBuilt();
    }
}