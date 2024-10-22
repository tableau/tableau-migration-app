// <copyright file="UserDomainMappingViewModel.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.ViewModels;
using Microsoft.Extensions.Options;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.GUI.Models;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class UserDomainMappingViewModel : ValidatableViewModelBase
{
    private readonly IOptions<EmailDomainMappingOptions> emailDomainOptions;
    private string cloudUserDomain = string.Empty;
    private bool isMappingDisabled = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDomainMappingViewModel" /> class.
    /// </summary>
    /// <param name="options">Email Domain options to be used for setting Default domain hook.</param>
    public UserDomainMappingViewModel(IOptions<EmailDomainMappingOptions> options)
    {
        this.emailDomainOptions = options;
    }

    /// <summary>
    /// Gets or sets the default cloud domain mapping for users.
    /// </summary>
    public string CloudUserDomain
    {
        get => this.cloudUserDomain;
        set
        {
            this.SetProperty(ref this.cloudUserDomain, value);
            this.ValidateCloudUserDomain();
            this.emailDomainOptions.Value.EmailDomain = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the mapping is disabled.
    /// </summary>
    public bool IsMappingDisabled
    {
        get => this.isMappingDisabled;
        set
        {
            this.SetProperty(ref this.isMappingDisabled, value);
            this.ValidateAll();
        }
    }

    /// <inheritdoc/>
    public override void ValidateAll()
    {
        this.ValidateCloudUserDomain();
    }

    private void ValidateCloudUserDomain()
    {
        const string requiredMessage = "Tableau Server to Cloud User Domain Mapping is required.";
        const string validityMessage = "The provided value is not a valid domain.";
        if (this.IsMappingDisabled)
        {
            this.RemoveError(nameof(this.CloudUserDomain), requiredMessage);
            this.RemoveError(nameof(this.CloudUserDomain), validityMessage);
            return;
        }

        this.ValidateRequired(
            this.CloudUserDomain,
            nameof(this.CloudUserDomain),
            requiredMessage);

        // Validate the domain
        if (!Validator.IsDomainNameValid(this.CloudUserDomain))
        {
            this.AddError(nameof(this.CloudUserDomain), validityMessage);
        }
        else
        {
            this.RemoveError(nameof(this.CloudUserDomain), validityMessage);
        }
    }
}