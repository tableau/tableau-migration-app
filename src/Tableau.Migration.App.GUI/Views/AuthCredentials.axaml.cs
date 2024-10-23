// <copyright file="AuthCredentials.axaml.cs" company="Salesforce, Inc.">
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
using System.Reactive.Linq;
using Tableau.Migration.App.GUI.Models;

/// <summary>
/// View for Tableau Server/Cloud credentials.
/// </summary>
public partial class AuthCredentials : UserControl
{
    /// <summary>
    /// The Tableau environment property.
    /// </summary>
    public static readonly StyledProperty<TableauEnv> TableauEnvProperty =
        AvaloniaProperty.Register<AuthCredentials, TableauEnv>(nameof(TableauEnv));

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthCredentials" /> class.
    /// </summary>
    public AuthCredentials()
    {
        this.InitializeComponent();
        this.GetObservable(TableauEnvProperty)
            .ForEachAsync(newValue => this.SetSubProperties());
    }

    /// <summary>
    /// Gets or sets a value indicating whether the environment is a Tableau Server or Tableau Cloud.
    /// </summary>
    public TableauEnv TableauEnv
    {
        get => this.GetValue(TableauEnvProperty);
        set => this.SetValue(TableauEnvProperty, value);
    }

    private void SetSubProperties()
    {
        this.UriDetails.TableauEnv = this.TableauEnv;
        this.TokenDetails.TableauEnv = this.TableauEnv;
        return;
    }
}