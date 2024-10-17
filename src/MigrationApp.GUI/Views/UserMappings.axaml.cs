// <copyright file="UserMappings.axaml.cs" company="Salesforce, Inc.">
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
using Avalonia.Controls;

/// <summary>
/// View for Tableau Server to Cloud User Mappings.
/// </summary>
public partial class UserMappings : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMappings" /> class.
    /// </summary>
    public UserMappings()
    {
        this.InitializeComponent();
    }
}