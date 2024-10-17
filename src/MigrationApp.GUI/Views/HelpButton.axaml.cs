// <copyright file="HelpButton.axaml.cs" company="Salesforce, Inc.">
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

namespace MigrationApp.GUI.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;

/// <summary>
/// A help button component providing information for the associated fields.
/// </summary>
public partial class HelpButton : UserControl
{
    /// <summary>
    /// The text associated with the contents of this button.
    /// </summary>
    public static readonly StyledProperty<string> HelpTextProperty =
        AvaloniaProperty.Register<HelpButton, string>(nameof(HelpText));

    /// <summary>
    /// The URL associated with the contents of the button.
    /// </summary>
    public static readonly StyledProperty<string> DetailsUrlProperty =
        AvaloniaProperty.Register<HelpButton, string>(nameof(DetailsUrl));

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpButton"/> class.
    /// Creates a new <see cref="HelpButton" /> object.
    /// </summary>
    public HelpButton()
    {
        this.DataContext = this;
        this.InitializeComponent();

        this.Initialized += (sender, e) =>
        {
            var linkTextBlock = this.FindControl<TextBlock>("LinkTextBlock");
            if (linkTextBlock != null)
            {
                linkTextBlock.PointerPressed += this.OnLinkClicked;
            }
        };
    }

    /// <summary>
    /// Gets or Sets the URL to be used in the link of the help content.
    /// </summary>
    public string DetailsUrl
    {
        get => this.GetValue(DetailsUrlProperty);
        set => this.SetValue(DetailsUrlProperty, value);
    }

    /// <summary>
    /// Gets or Sets the text content for the help button.
    /// </summary>
    public string HelpText
    {
        get => this.GetValue(HelpTextProperty);
        set => this.SetValue(HelpTextProperty, value);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnLinkClicked(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(this.DetailsUrl))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = this.DetailsUrl,
                    UseShellExecute = true,
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open URL: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("DetailsUrl is not set or empty.");
        }
    }
}