// <copyright file="MainWindow.axaml.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MigrationApp.GUI.Views;

using Avalonia.Controls;
using MigrationApp.GUI.ViewModels;
using System;

/// <summary>
/// The main window for the application.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
    }

    private async void StopMigrationOnClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var confirmationDialog = new ConfirmationDialog(
            "Stop Migration",
            "Are you sure you want to stop the migration?",
            "Stop Migration",
            "Cancel");

        var result = await confirmationDialog.ShowDialog<bool>(this);

        if (result)
        {
            // Handle OK
            MainWindowViewModel? myViewModel = this.DataContext as MainWindowViewModel;
            myViewModel?.CancelMigration();
        }
    }
}