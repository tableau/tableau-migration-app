// <copyright file="UriDetailsViewModel.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.GUI.ViewModels;
using MigrationApp.GUI.Models;
using System;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class UriDetailsViewModel
    : ValidatableViewModelBase
{
    private string uriFull = string.Empty;
    private string envLabel = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="UriDetailsViewModel" /> class.
    /// </summary>
    /// <param name="env">The Tableau environment.</param>
    public UriDetailsViewModel(TableauEnv env = TableauEnv.TableauServer)
    {
        this.envLabel = TableauEnvMapper.ToString(env);

        this.UriBaseDefaultMessage = $"No Tableau {this.envLabel} URI has been provided.";
    }

    /// <summary>
    /// Gets or sets the full URI of the Tableau environment.
    /// </summary>
    public string UriFull
    {
        get => this.uriFull;
        set
        {
            this.SetProperty(ref this.uriFull, value);
            this.ValidateAll();
            this.UriBase = this.ExtractBaseUri(value);
            this.OnPropertyChanged(nameof(this.UriBase));
            this.SiteContent = this.ExtractSiteContent(value);
            this.OnPropertyChanged(nameof(this.SiteContent));
        }
    }

    /// <summary>
    /// Gets or sets the URI Base of the Tableau environment.
    /// </summary>
    public string UriBase { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Default message to be returned if no URI Base is set.
    /// </summary>
    public string UriBaseDefaultMessage { get; set; }

    /// <summary>
    /// Gets or sets the URI Site Name.
    /// </summary>
    public string SiteContent { get; set; } = "Default site is selected.";

    /// <inheritdoc/>
    public override void ValidateAll()
    {
        this.ValidateUri();
    }

    private void ValidateUri()
    {
        this.ValidateRequired(this.UriFull, nameof(this.UriFull), $"The Tableau {this.envLabel} URI is required.");
        this.ValidateUriFormat(this.UriFull, nameof(this.UriFull), $"Invalid URI Format for the Tableau {this.envLabel} URI.");
    }

    private void ValidateUriFormat(string value, string propertyName, string errorMessage)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            this.AddError(propertyName, errorMessage);
        }
        else
        {
            this.RemoveError(propertyName, errorMessage);
        }
    }

    private string ExtractBaseUri(string uri)
    {
        if (Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
        {
            return $"{parsedUri.Scheme}://{parsedUri.Host}/";
        }

        return string.Empty;
    }

    private string ExtractSiteContent(string uri)
    {
        if (Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
        {
            var fragment = parsedUri.Fragment;

            var fragmentSegments = fragment.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < fragmentSegments.Length; i++)
            {
                if (fragmentSegments[i].Equals(
                        "site",
                        StringComparison.OrdinalIgnoreCase)
                    && i + 1 < fragmentSegments.Length)
                {
                    return fragmentSegments[i + 1];
                }
            }
        }

        return string.Empty;
    }
}