// <copyright file="UriDetails.axaml.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Reactive.Linq;
using Tableau.Migration.App.GUI.Models;

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
        this.InfoHelp.HelpText = string.Format(ViewConstants.URIDetailsHelpTextTemplate, env, lowerEnv);
        this.InfoHelp.DetailsUrl = "https://help.tableau.com/current/pro/desktop/en-us/embed_structure.htm";
        this.UriFull.Watermark = $"Ex: http://<{lowerEnv}_address>/#/site/<site_name>";

        this.BaseUriLabel.Text = $"{env} Base URI";
        this.SiteNameLabel.Text = $"{env} Site Name";
    }
}