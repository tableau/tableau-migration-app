// <copyright file="AuthCredentialsViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
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