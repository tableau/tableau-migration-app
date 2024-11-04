// <copyright file="SplitButton.axaml.cs" company="Salesforce, Inc.">
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
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Windows.Input;
using Tableau.Migration.App.GUI.ViewModels;

/// <summary>
/// Split Button component.
/// </summary>
public partial class SplitButton : UserControl
{
    /// <summary>
    /// Dependency property to store the command for primary command.
    /// </summary>
    public static readonly StyledProperty<ICommand> PrimaryCommandProperty =
        AvaloniaProperty.Register<SplitButton, ICommand>(nameof(PrimaryCommand));

    /// <summary>
    /// Dependency property to store the command for secondary command.
    /// </summary>
    public static readonly StyledProperty<ICommand> SecondaryCommandProperty =
        AvaloniaProperty.Register<SplitButton, ICommand>(nameof(SecondaryCommand));

    /// <summary>
    /// Dependency property to set the primary button text.
    /// </summary>
    public static readonly StyledProperty<string> PrimaryButtonTextProperty =
        AvaloniaProperty.Register<SplitButton, string>(nameof(PrimaryButtonText), "Primary Action");

    /// <summary>
    /// Dependency property to set the secondary button text.
    /// </summary>
    public static readonly StyledProperty<string> SecondaryButtonTextProperty =
        AvaloniaProperty.Register<SplitButton, string>(nameof(SecondaryButtonText), "Secondary Action");

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitButton"/> class.
    /// </summary>
    public SplitButton()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the command for primary action.
    /// </summary>
    public ICommand PrimaryCommand
    {
        get => this.GetValue(PrimaryCommandProperty);
        set => this.SetValue(PrimaryCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command for secondary action.
    /// </summary>
    public ICommand SecondaryCommand
    {
        get => this.GetValue(SecondaryCommandProperty);
        set => this.SetValue(SecondaryCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the text displayed on the main button.
    /// </summary>
    public string PrimaryButtonText
    {
        get => this.GetValue(PrimaryButtonTextProperty);
        set => this.SetValue(PrimaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text displayed on the menu item.
    /// </summary>
    public string SecondaryButtonText
    {
        get => this.GetValue(SecondaryButtonTextProperty);
        set => this.SetValue(SecondaryButtonTextProperty, value);
    }
}