// <copyright file="MainWindow.axaml.cs" company="Salesforce, Inc.">
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
using MigrationApp.GUI.ViewModels;

/// <summary>
/// The main window for the application.
/// </summary>
public partial class MainWindow : Window
{
    private bool isDialogOpen = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        this.Closing += this.OnWindowClosing;
    }

    private async void StopMigrationOnClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var confirmationStopDialog = new ConfirmationDialog(
            "Stop Migration",
            "Are you sure you want to stop the migration?",
            "Stop Migration",
            "Cancel");

        var result = await confirmationStopDialog.ShowDialog<bool>(this);

        if (result)
        {
            // Handle OK
            MainWindowViewModel? myViewModel = this.DataContext as MainWindowViewModel;
            myViewModel?.CancelMigration();
        }
    }

    // Override the default closing behavior to prompt when a migration is ongoing
    private async void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // Prevent closing while dialog is open
        if (this.isDialogOpen)
        {
            e.Cancel = true;
            return;
        }

        // Get the ViewModel from the DataContext
        if (this.DataContext is MainWindowViewModel viewModel)
        {
            // Check if there is an ongoing migration
            if (viewModel.IsMigrating)
            {
                // Prevent window from immediately closing
                e.Cancel = true;
                this.isDialogOpen = true;

                // Open confirmation window
                var confirmationExitDialog = new ConfirmationDialog(
                    "Quit",
                    "A migration is running! Are you sure you want to stop the migration and exit?",
                    "Stop Migration and Exit",
                    "Cancel");

                var result = await confirmationExitDialog.ShowDialog<bool>(this);
                this.isDialogOpen = false;

                if (result)
                {
                    // Cancel migration and then exit
                    viewModel.CancelMigration();

                    // Defer a closing call because the current close event has been cancelled
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        this.Closing -= this.OnWindowClosing; // Unsubscribe to prevent recursion
                        this.Close();
                    });
                }
            }
        }
    }
}