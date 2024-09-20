// <copyright file="HelpButton.axaml.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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