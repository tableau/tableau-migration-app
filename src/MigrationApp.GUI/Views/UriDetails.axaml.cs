// <copyright file="UriDetails.axaml.cs" company="Salesforce, inc">
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
public partial class UriDetails : UserControl
{
    /// <summary>
    /// The Tableau environment property.
    /// </summary>
    public static readonly StyledProperty<TableauEnv> TableauEnvProperty =
        AvaloniaProperty.Register<UriDetails, TableauEnv>(nameof(TableauEnv));

    /// <summary>
    /// Initializes a new instance of the <see cref="UriDetails" /> class.
    /// </summary>
    public UriDetails()
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

        string lowerEnv = env.ToLower();

        this.Header.Text = $"Tableau {env}";

        // Full URI Input
        this.UriLabel.Text = $"Tableau {env} URI";
        this.InfoHelp.HelpText =
            $"Enter the Tableau {env} URL in one of the following formats:"
            + Environment.NewLine +
            $"- For a single-site: http://<{lowerEnv}_address>"
            + Environment.NewLine +
            $"- For a multi-site: http://<{lowerEnv}_address>/#/site/<site_name>"
            + Environment.NewLine +
            "The site name is parsed from the URL if one is provided.";
        this.InfoHelp.DetailsUrl = "https://help.tableau.com/current/pro/desktop/en-us/embed_structure.htm";
        this.UriFull.Watermark = $"Ex: http://<{lowerEnv}_address>/#/site/<site_name>";

        this.BaseUriLabel.Text = $"{env} Base URI";
        this.SiteNameLabel.Text = $"{env} Site Name";
    }
}