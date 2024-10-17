// <copyright file="EndPointOptions.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.Core.Entities;

/// <summary>
/// Defines the endpoint access information for migration.
/// </summary>
public class EndpointOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointOptions"/> class.
    /// </summary>
    public EndpointOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointOptions"/> class.
    /// </summary>
    /// <param name="url">The URL to connect to.</param>
    /// <param name="siteContentUrl">The Tableau Site to connect to on the host URL.</param>
    /// <param name="accessTokenName">The name of the Personal Access Token to use for access.</param>
    /// <param name="accessToken">The Personal Access Token to use for access.</param>
    public EndpointOptions(string url, string siteContentUrl, string accessTokenName, string accessToken)
    {
        this.Url = new Uri(url);
        this.SiteContentUrl = siteContentUrl;
        this.AccessTokenName = accessTokenName;
        this.AccessToken = accessToken;
    }

    /// <summary>
    ///  Gets or Sets the URI to connect to.
    /// </summary>
    public Uri Url { get; set; } = new Uri("https://default.tableau.cloud");

    /// <summary>
    /// Gets or sets the site to target.
    /// </summary>
    public string SiteContentUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the Personal Access Token to be used for the connection.
    /// </summary>
    public string AccessTokenName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or Sets the Personal Access Token to be used for the connection.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Validates the provided endpoint data.
    /// </summary>
    /// <returns>Whether or not the endpoint data is valid.</returns>
    public bool IsValid()
    {
        return this.Url != null && !string.IsNullOrEmpty(this.AccessTokenName) && !string.IsNullOrEmpty(this.AccessToken);
    }
}