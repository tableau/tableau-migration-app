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

namespace Tableau.Migration.App.GUI.Views;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tableau.Migration.App.GUI.ViewModels;

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
        await this.HandleMigrationCancellation("Stop Migration", "Are you sure you want to stop the migration?");
    }

    private async void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (this.isDialogOpen)
        {
            e.Cancel = true;
            return;
        }

        if (this.DataContext is MainWindowViewModel viewModel)
        {
            if (viewModel.IsMigrating)
            {
                e.Cancel = true;
                this.isDialogOpen = true;

                await this.HandleMigrationCancellation("Quit", "A migration is running! Are you sure you want to stop the migration and exit?");

                this.isDialogOpen = false;

                if (!viewModel.IsMigrating)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        this.Closing -= this.OnWindowClosing;
                        this.Close();
                    });
                }
            }
        }
    }

    private async Task HandleMigrationCancellation(string title, string message)
    {
        var confirmationDialog = new ConfirmationDialog(
            title,
            message,
            "Stop",
            "Cancel");

        var result = await confirmationDialog.ShowDialog<bool>(this);

        if (result)
        {
            var saveManifestDialog = new ConfirmationDialog(
                "Save Manifest File",
                "Do you want to save the manifest to resume migration later?",
                "Yes",
                "No");

            var resultSaveManifest = await saveManifestDialog.ShowDialog<bool>(this);
            MainWindowViewModel? myViewModel = this.DataContext as MainWindowViewModel;
            if (resultSaveManifest)
            {
                var window = this;
                var options = new FilePickerSaveOptions
                {
                    Title = "Save Manifest",
                    FileTypeChoices = new List<FilePickerFileType>
                    {
                        new FilePickerFileType("JSON files") { Patterns = new[] { "*.json" } },
                        new FilePickerFileType("All files") { Patterns = new[] { "*.*" } },
                    },
                    DefaultExtension = "json",
                };

                var resultFile = await window.StorageProvider.SaveFilePickerAsync(options);

                if (resultFile != null)
                {
                    var filePath = resultFile.TryGetLocalPath();
                    myViewModel?.CancelMigration(filePath ?? string.Empty);
                }
                else
                {
                    myViewModel?.CancelMigration(string.Empty);
                }
            }
            else
            {
                myViewModel?.CancelMigration(string.Empty);
            }
        }
    }
}