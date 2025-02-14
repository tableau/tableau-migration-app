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

namespace Tableau.Migration.App.GUI.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Tableau.Migration.App.GUI.ViewModels;

/// <summary>
/// View control for Message Display.
/// </summary>
public partial class MessageDisplay : UserControl
{
    private ScrollViewer? scrollViewer;
    private ILogger<MessageDisplay>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDisplay" /> class.
    /// </summary>
    public MessageDisplay()
    {
        this.InitializeComponent();
        this.DataContext = App.ServiceProvider?.GetRequiredService<MessageDisplayViewModel>();
        this.logger = App.ServiceProvider?.GetRequiredService<ILogger<MessageDisplay>>();

        // Setup for "Scroll to bottom" behaviour
        // Wait for component to load, then go find the ScrollViewer
        this.KeepScrolledCheckBox.AttachedToVisualTree += this.OnKeepScrolledCheckBoxAttached;
        this.KeepScrolledCheckBox.IsChecked = true; // Set Initial state to keep scrolled
        this.KeepScrolledCheckBox.PropertyChanged += this.KeepScrolled_PropertyChanged;

        this.MessageTextBox.PropertyChanged += this.TextBox_PropertyChanged;
    }

    private void OnKeepScrolledCheckBoxAttached(
        object? sender, VisualTreeAttachmentEventArgs e)
    {
        // Remove the subscription as we only need to evalute this once.
        this.KeepScrolledCheckBox.AttachedToVisualTree -= this.OnKeepScrolledCheckBoxAttached;

        // Use Dispatcher to delay the ScrollViewer binding until the visual tree is ready
        Dispatcher.UIThread.Post(
            () =>
            {
                // Attempt to find the ScrollViewer within the Message TextBox
                this.scrollViewer = this.MessageTextBox.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

                if (this.scrollViewer != null)
                {
                    // Subscribe to PropertyChanged to attach callbacks on scroll
                    this.scrollViewer.PropertyChanged += this.ScrollViewer_PropertyChanged;
                }
                else
                {
                    this.logger?.LogDebug("ScrollViewer not found within the TextBox.");
                }
            }, DispatcherPriority.Loaded);
    }

    private void ScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        // Uncheck the 'keep scrolled to bottom' checkbox if the user scrolls up
        if (e.Property == ScrollViewer.OffsetProperty)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null &&
                e.OldValue is Vector oldOffset &&
                e.NewValue is Vector newOffset &&
                oldOffset.Y > newOffset.Y)
            {
                if (this.KeepScrolledCheckBox?.IsChecked ?? false)
                {
                    this.KeepScrolledCheckBox.IsChecked = false;
                }
            }
        }
    }

    private void KeepScrolled_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CheckBox.IsCheckedProperty)
        {
            this.ScrollToBottom();
        }
    }

    private void ScrollToBottom()
    {
        if (this.KeepScrolledCheckBox?.IsChecked ?? false)
        {
            // Set to dispatcher so that the scrolling happens after other UI updates
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (this.scrollViewer != null)
                    {
                        // Scroll to the bottom by setting the VerticalOffset to the maximum height
                        this.scrollViewer.Offset = new Vector(this.scrollViewer.Offset.X, this.scrollViewer.Extent.Height);
                    }
                    else
                    {
                        // Manipulate the TextBox caret if ScrollViewer is not available
                        this.MessageTextBox.CaretIndex = this.MessageTextBox.Text?.Length ?? 0;
                        this.MessageTextBox.SelectionStart = this.MessageTextBox.Text?.Length ?? 0;
                        this.MessageTextBox.SelectionEnd = this.MessageTextBox.Text?.Length ?? 0;
                    }
                }, DispatcherPriority.Background);
        }
    }

    private void TextBox_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == TextBox.TextProperty)
        {
            this.ScrollToBottom();
        }
    }
}