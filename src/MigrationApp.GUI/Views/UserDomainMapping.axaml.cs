// <copyright file="UserDomainMapping.axaml.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Views;
using Avalonia.Controls;

/// <summary>
/// View for User Domain Mappings.
/// </summary>
public partial class UserDomainMapping : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDomainMapping" /> class.
    /// </summary>
    public UserDomainMapping()
    {
        this.InitializeComponent();

        // Attach an event callback when to Checkbox evetns to disable the Textbox
        this.DisableMapping.PropertyChanged += this.CheckBox_PropertyChanged;
    }

    private void CheckBox_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CheckBox.IsCheckedProperty)
        {
            this.UserCloudDomain.IsEnabled = !this.DisableMapping.IsChecked ?? true;
        }
    }
}