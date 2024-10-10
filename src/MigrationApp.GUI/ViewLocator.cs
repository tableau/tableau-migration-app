// <copyright file="ViewLocator.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MigrationApp.GUI.ViewModels;
using System;

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