// <copyright file="DictionaryUserMapping.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.Core.Hooks.Mappings;

using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Options;
using Tableau.Migration.Resources;

/// <summary>
/// Specific user mappings set from a defined Tableau Server User to Tableau Cloud User Dictionary.
/// </summary>
public class DictionaryUserMapping :
    ContentMappingBase<IUser>, // Base class to build mappings for content types
    ITableauCloudUsernameMapping
{
    private readonly Dictionary<
        string /* Tableau Server Username*/, string /* Tableau Cloud Username*/> userMappings;

    private ILogger<DictionaryUserMapping> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryUserMapping"/> class.
    /// Creates a new <see cref="DictionaryUserMapping"/> object.
    /// </summary>
    /// <param name="optionsProvider">The options for this Mapping.</param>
    /// <param name="localizer">The localizer for this Mapping.</param>
    /// <param name="logger">The logger for this Mapping.</param>
    public DictionaryUserMapping(
        IMigrationPlanOptionsProvider<DictionaryUserMappingOptions> optionsProvider,
        ISharedResourcesLocalizer localizer,
        ILogger<DictionaryUserMapping> logger)
        : base(localizer, logger)
    {
        this.userMappings = optionsProvider.Get().UserMappings;
        this.logger = logger;
    }

    /// <summary>
    /// Maps a user based on the defined mappings found in the provided dictionary..
    /// </summary>
    /// <param name="userMappingContext">The context from which to set the user mapping information.</param>
    /// <param name="cancel">The cancellation token to interrupt the mapping process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override Task<ContentMappingContext<IUser>?> MapAsync(
        ContentMappingContext<IUser> userMappingContext,
        CancellationToken cancel)
    {
        Tableau.Migration.ContentLocation user = userMappingContext.MappedLocation.Parent();

        var domain = userMappingContext.MappedLocation.Parent();

        // Only map users that are defined in the dictionary
        if (!this.userMappings.ContainsKey(userMappingContext.ContentItem.Name))
        {
            this.logger.LogInformation("{user} not found in user provided mapping.", userMappingContext.ContentItem.Name);
            return userMappingContext.ToTask();
        }

        try
        {
            MailAddress mailAddress = new MailAddress(this.userMappings[userMappingContext.ContentItem.Name]);
            this.logger.LogInformation(
                "{user} mapped as {newUser}",
                userMappingContext.ContentItem.Name,
                this.userMappings[userMappingContext.ContentItem.Name]);
            return userMappingContext.MapTo(
                domain.Append(
                    this.userMappings[userMappingContext.ContentItem.Name])).ToTask();
        }
        catch (FormatException)
        {
            this.logger.LogInformation(
                "CSV User mapping for {serverUsername} not in email format: [{cloudUsername}]. Skipping.",
                userMappingContext.ContentItem.Name,
                this.userMappings[userMappingContext.ContentItem.Name]);
            return userMappingContext.ToTask();
        }
    }
}