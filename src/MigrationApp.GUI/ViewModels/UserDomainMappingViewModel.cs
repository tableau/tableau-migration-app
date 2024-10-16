// <copyright file="UserDomainMappingViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;
using Microsoft.Extensions.Options;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.GUI.Models;

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