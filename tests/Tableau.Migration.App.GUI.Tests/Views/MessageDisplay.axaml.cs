// <copyright file="MessageDisplay.axaml.cs" company="Salesforce, Inc.">
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

namespace MessageDisplayTest;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Text;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.Views;
using Xunit;

public class MessageDisplayTest
{
    [AvaloniaFact]
    public void MessageDisplay_InitialState()
    {
        var messageDisplay = new MessageDisplay();
        Assert.NotNull(messageDisplay);

        var messageTextBox = messageDisplay.FindControl<TextBox>("MessageTextBox");
        Assert.NotNull(messageTextBox);

        var keepScrolledCheckBox = messageDisplay.FindControl<CheckBox>("KeepScrolledCheckBox");
        Assert.NotNull(keepScrolledCheckBox);
        Assert.True(keepScrolledCheckBox.IsChecked);
    }

    [AvaloniaFact]
    public void MessageDisplay_UnCheckCheckBoxOnScroll()
    {
        var messageDisplay = new MessageDisplay();

        var messageTextBox = messageDisplay.FindControl<TextBox>("MessageTextBox");

        var keepScrolledCheckBox = messageDisplay.FindControl<CheckBox>("KeepScrolledCheckBox");
        Assert.NotNull(keepScrolledCheckBox);
        Assert.NotNull(messageTextBox);
        Assert.True(keepScrolledCheckBox!.IsChecked);
        messageTextBox!.TemplateApplied += (sender, e) =>
        {
            var scrollViewer = messageTextBox!.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .FirstOrDefault();

            Assert.NotNull(scrollViewer);
            if (scrollViewer != null)
            {
                scrollViewer.Offset = new Vector(0, -20); // Scroll down by 20 units.
            }

            Assert.False(keepScrolledCheckBox!.IsChecked);
        };

        messageTextBox!.ApplyTemplate();
    }
}