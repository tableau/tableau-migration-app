// <copyright file="AuthCredentials.axaml.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MigrationApp.GUI.Models;
using System.Reactive.Linq;

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