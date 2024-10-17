// <copyright file="ConfirmationDialog.axaml.cs" company="Salesforce, Inc.">
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
using Avalonia.Markup.Xaml;

/// <summary>
/// Customizable confirmation pop up dialog window.
/// </summary>
public partial class ConfirmationDialog : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmationDialog"/> class.
    /// Initializes an instance of the <see cref="ConfirmationDialog" /> class.
    /// </summary>
    public ConfirmationDialog()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmationDialog"/> class.
    /// Initializes an instance of the <see cref="ConfirmationDialog" /> class.
    /// </summary>
    /// <param name="title">The title for the dialog.</param>
    /// <param name="message">The message body of the dialog.</param>
    /// <param name="okContent">The text displayed on the confirmation button.</param>
    /// <param name="cancelContent">The text displayed on the cancellation button.</param>
    public ConfirmationDialog(
        string title,
        string message,
        string okContent,
        string cancelContent)
    {
        this.InitializeComponent();
        this.Title = title;
        var messageTextBlock = this.FindControl<TextBlock>("MessageTextBlock");
        var okButton = this.FindControl<Button>("OkButton");
        var cancelButton = this.FindControl<Button>("CancelButton");

        // Set Dialog contents
        messageTextBlock!.Text = message;
        okButton!.Content = okContent;
        cancelButton!.Content = cancelContent;

        // Set button handlers
        okButton!.Click += this.OkButtonClick;
        cancelButton!.Click += this.CancelButtonClick;
    }

    /// <summary>
    /// Gets the result of the confirmation dialog.
    /// <c>true</c> if the user confirmed; <c>false</c> if the user canceled.
    /// </summary>
    public bool? Result { get; private set; }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OkButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Result = true;
        this.Close(this.Result);  // Return true when OK is clicked
    }

    private void CancelButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Result = false;
        this.Close(this.Result);  // Return false when Cancel is clicked
    }
}