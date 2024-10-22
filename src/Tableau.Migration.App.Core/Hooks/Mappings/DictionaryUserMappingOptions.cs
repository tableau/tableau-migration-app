// <copyright file="DictionaryUserMappingOptions.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.Core.Hooks.Mappings
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