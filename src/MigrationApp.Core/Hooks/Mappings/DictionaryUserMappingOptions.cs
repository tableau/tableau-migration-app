// <copyright file="DictionaryUserMappingOptions.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Core.Hooks.Mappings
{
    /// <summary>
    /// Mapping options for user specific migration by Dictionary.
    /// </summary>
    public sealed class DictionaryUserMappingOptions
    {
        /// <summary>
        /// Gets or Sets the mapping Dictionary defining the migration user names.
        /// </summary>
        public Dictionary<string, string> UserMappings { get; set; } = new ();
    }
}