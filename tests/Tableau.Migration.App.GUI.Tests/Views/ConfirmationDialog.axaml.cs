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

namespace ConfirmationDialogTest;

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Tableau.Migration.App.GUI.Views;
using Xunit;

public class ConfirmationDialogTest
{
    [AvaloniaFact]
    public void ConfirmationDialog_InitialState()
    {
        var dialog = new ConfirmationDialog("titletest", "messagetest", "OKtest", "Canceltest");
        var messageTextBlock = dialog.FindControl<TextBlock>("MessageTextBlock");
        var okButton = dialog.FindControl<Button>("OkButton");
        var cancelButton = dialog.FindControl<Button>("CancelButton");

        Assert.NotNull(messageTextBlock);
        Assert.NotNull(okButton);
        Assert.NotNull(cancelButton);

        Assert.Equal("titletest", dialog.Title);
        Assert.Equal("messagetest", messageTextBlock!.Text);
        Assert.Equal("OKtest", okButton!.Content);
        Assert.Equal("Canceltest", cancelButton!.Content);
    }

    [AvaloniaFact]
    public void ConfirmationDialog_InitialState_EmptyCtr()
    {
        var dialog = new ConfirmationDialog();
        var messageTextBlock = dialog.FindControl<TextBlock>("MessageTextBlock");
        var okButton = dialog.FindControl<Button>("OkButton");
        var cancelButton = dialog.FindControl<Button>("CancelButton");

        Assert.Equal("Confirmation", dialog.Title);
        Assert.NotNull(messageTextBlock);
        Assert.NotNull(okButton);
        Assert.NotNull(cancelButton);
        Assert.Null(messageTextBlock!.Text);
        Assert.Null(okButton!.Content);
        Assert.Null(cancelButton!.Content);
    }

    [AvaloniaFact]
    public void ConfirmationDialog_OnClickOK()
    {
        var dialog = new ConfirmationDialog("title", "message", "ok", "cancel");
        var okButton = dialog.FindControl<Button>("OkButton");

        okButton!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.True(dialog.Result);
    }

    [AvaloniaFact]
    public void ConfirmationDialog_OnClickCancel()
    {
        var dialog = new ConfirmationDialog("title", "message", "ok", "cancel");
        var cancelButton = dialog.FindControl<Button>("CancelButton");
        cancelButton!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.False(dialog.Result);
    }
}