// <copyright file="ViewLocator.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using Tableau.Migration.App.GUI.ViewModels;

/// <summary>
/// Class to retrieve the associated View of a Viewmodel.
/// </summary>
public class ViewLocator : IDataTemplate
{
    /// <summary>
    /// Creates a View from the provided view model data.
    /// </summary>
    /// <param name="data">The ViewModel object.</param>
    /// <returns>Either the associated View if one exists, or else a "Not Found" Textblock if not.</returns>
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type) !;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    /// <summary>
    /// Determins whether or not a provided object is a <see cref="ViewModelBase" />.
    /// </summary>
    /// <param name="data">The object to check.</param>
    /// <returns>Whether or not the data is a <see cref="ViewModelBase" />.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}