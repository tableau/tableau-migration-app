// <copyright file="TokenDetailsViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;
using MigrationApp.GUI.Models;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class TokenDetailsViewModel
   : ValidatableViewModelBase
{
    private string tokenName = string.Empty;
    private string tokenSecret = string.Empty;
    private string envLabel = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenDetailsViewModel" /> class.
    /// </summary>
    /// <param name="env">The Tableau environment.</param>
    public TokenDetailsViewModel(TableauEnv env = TableauEnv.TableauServer)
    {
        this.envLabel = TableauEnvMapper.ToString(env);
    }

    /// <summary>
    /// Gets or sets the Personal Access Token Name.
    /// </summary>
    public string TokenName
    {
        get => this.tokenName;
        set
        {
            this.SetProperty(ref this.tokenName, value);
            this.ValidateTokenName();
        }
    }

    /// <summary>
    /// Gets or sets the Personal Access Token Secret.
    /// </summary>
    public string TokenSecret
    {
        get => this.tokenSecret;
        set
        {
            this.SetProperty(ref this.tokenSecret, value);
            this.ValidateTokenSecret();
        }
    }

    /// <inheritdoc/>
    public override void ValidateAll()
    {
        this.ValidateTokenName();
        this.ValidateTokenSecret();
        return;
    }

    private void ValidateTokenName()
    {
        this.ValidateRequired(this.TokenName, nameof(this.TokenName), $"Tableau {this.envLabel} Access Token Name is required.");
    }

    private void ValidateTokenSecret()
    {
        this.ValidateRequired(this.TokenSecret, nameof(this.TokenSecret), $"Tableau {this.envLabel} Access Token Secret is required.");
    }
}