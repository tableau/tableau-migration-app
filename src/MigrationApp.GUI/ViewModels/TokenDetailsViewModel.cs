// <copyright file="TokenDetailsViewModel.cs" company="Salesforce, Inc.">
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