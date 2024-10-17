// <copyright file="AuthCredentialsViewModel.cs" company="Salesforce, Inc.">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ViewModel for the URI Details component.
/// </summary>
public partial class AuthCredentialsViewModel
   : ValidatableViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthCredentialsViewModel" /> class.
    /// </summary>
    /// <param name="env">The Tableau Environment.</param>
    public AuthCredentialsViewModel(TableauEnv env = TableauEnv.TableauServer)
    {
        this.UriDetailsVM = new UriDetailsViewModel(env);
        this.TokenDetailsVM = new TokenDetailsViewModel(env);
    }

    /// <summary>
    /// Gets the viewmodel for the URI details.
    /// </summary>
    public UriDetailsViewModel UriDetailsVM { get; }

    /// <summary>
    /// Gets the viewmodel for the PAT details.
    /// </summary>
    public TokenDetailsViewModel TokenDetailsVM { get; }

    /// <inheritdoc/>
    public override void ValidateAll()
    {
        this.UriDetailsVM.ValidateAll();
        this.TokenDetailsVM.ValidateAll();
    }

    /// <inheritdoc/>
    public override int GetErrorCount()
    {
        return
            base.GetErrorCount()
            + this.TokenDetailsVM.GetErrorCount()
            + this.UriDetailsVM.GetErrorCount();
    }

    /// <inheritdoc/>
    public override IEnumerable GetErrors(string? propertyName)
    {
        List<string> errors = new List<string>();
        errors.AddRange(base.GetErrors(propertyName).Cast<string>());
        errors.AddRange(this.UriDetailsVM.GetErrors(propertyName).Cast<string>());
        errors.AddRange(this.TokenDetailsVM.GetErrors(propertyName).Cast<string>());
        return errors;
    }
}