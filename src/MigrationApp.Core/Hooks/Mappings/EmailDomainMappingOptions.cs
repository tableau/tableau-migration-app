// <copyright file="EmailDomainMappingOptions.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Hooks.Mappings
{
    /// <summary>
    /// Mapping options for default domains to users without emails specified.
    /// </summary>
    public sealed class EmailDomainMappingOptions
    {
        /// <summary>
        /// Gets or Sets the default domain mappign to apply to users during migration.
        /// </summary>
        public string EmailDomain { get; set; } = string.Empty;
    }
}