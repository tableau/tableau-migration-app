// <copyright file="ConfirmationDialog.axaml.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

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