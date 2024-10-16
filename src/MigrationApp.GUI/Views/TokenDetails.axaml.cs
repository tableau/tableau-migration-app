// <copyright file="TokenDetails.axaml.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MigrationApp.GUI.Models;
using System;
using System.Reactive.Linq;

/// <summary>
/// View for Tableau Server/Cloud credentials.
/// </summary>
public partial class TokenDetails : UserControl
{
    /// <summary>
    /// The Tableau environment property.
    /// </summary>
    public static readonly StyledProperty<TableauEnv> TableauEnvProperty =
        AvaloniaProperty.Register<TokenDetails, TableauEnv>(nameof(TableauEnv));

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenDetails" /> class.
    /// </summary>
    public TokenDetails()
    {
        this.InitializeComponent();
        this.GetObservable(TableauEnvProperty)
            .ForEachAsync(newValue => this.SetLabels());
    }

    /// <summary>
    /// Gets or sets a value indicating whether the environment is a Tableau Server or Tableau Cloud.
    /// </summary>
    public TableauEnv TableauEnv
    {
        get => this.GetValue(TableauEnvProperty);
        set => this.SetValue(TableauEnvProperty, value);
    }

    private void SetLabels()
    {
        string env = TableauEnvMapper.ToString(this.TableauEnv);

        this.TokenNameLabel.Text = $"Tableau {env} PAT Name";
        this.TokenSecretLabel.Text = $"Tableau {env} PAT Secret";
        this.InfoHelp.HelpText =
            $"Personal Access Tokens (PATs) are used for authentication."
            + Environment.NewLine +
            "Enter the Personal Access Token Name."
            + Environment.NewLine +
            "Then, provide the Personal Access Token."
            + Environment.NewLine +
            $"Tokens can be managed in your Tableau {env} account's user settings.";
        this.InfoHelp.DetailsUrl = "https://help.tableau.com/current/online/en-us/security_personal_access_tokens.htm";
    }
}