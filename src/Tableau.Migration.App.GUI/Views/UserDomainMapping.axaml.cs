// <copyright file="UserDomainMapping.axaml.cs" company="Salesforce, Inc.">
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