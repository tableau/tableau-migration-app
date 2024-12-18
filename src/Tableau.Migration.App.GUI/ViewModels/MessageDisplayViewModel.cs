// <copyright file="MessageDisplayViewModel.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;

/// <summary>
/// ViewModel for the MessageDisplay component.
/// </summary>
public partial class MessageDisplayViewModel : ViewModelBase
{
    /// <summary>
    /// Maximum messages kept in the textbox before rotating out old messages.
    /// </summary>
    public const int MaxQueueSize = 1000;

    private readonly Queue<string> messageQueue;
    private readonly IProgressMessagePublisher publisher;
    private readonly ILogger<MessageDisplayViewModel>? logger;
    private bool isDetailsVisible = true;
    private string messages = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDisplayViewModel"/> class.
    /// </summary>
    /// <param name="publisher">The message publisher.</param>
    /// <param name="migrationTimer">The migration timer.</param>
    /// <param name="logger">The logger.</param>
    public MessageDisplayViewModel(
        IMigrationTimer migrationTimer,
        IProgressMessagePublisher publisher,
        ILogger<MessageDisplayViewModel>? logger)
    {
        this.Messages = "Migration output will be displayed here.";
        this.publisher = publisher;
        this.messageQueue = new Queue<string>();
        this.publisher.OnProgressMessage += this.HandleProgressMessage;
        this.logger = logger;
        this.ToggleDetailsCommand = new RelayCommand(this.ToggleDetails);
    }

    /// <summary>
    /// Gets or sets the message in the textbox.
    /// </summary>
    public string Messages
    {
        get => this.messages;
        set => this.SetProperty(ref this.messages, value);
    }

    /// <summary>
    /// Gets the text for the Show/Hide Details button based on the visibility state.
    /// </summary>
    public string ShowDetailsButtonText => this.IsDetailsVisible ? "Hide Details" : "Show Details";

    /// <summary>
    /// Gets command to toggle the visibility of the message details.
    /// </summary>
    public ICommand ToggleDetailsCommand { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the message details are visible.
    /// </summary>
    public bool IsDetailsVisible
    {
        get => this.isDetailsVisible;
        set
        {
            if (this.SetProperty(ref this.isDetailsVisible, value))
            {
                // Update the button text when the visibility changes
                this.OnPropertyChanged(nameof(this.ShowDetailsButtonText));
            }
        }
    }

    /// <summary>
    /// Adds a single line entry to the message queue.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void AddMessage(string message)
    {
        this.messageQueue.Enqueue(message);
        this.UpdateText();
    }

    /// <summary>
    /// Toggles the visibility of the message details.
    /// </summary>
    private void ToggleDetails()
    {
        this.IsDetailsVisible = !this.IsDetailsVisible;
    }

    private void HandleProgressMessage(ProgressEventArgs progressEvent)
    {
        // Enqueue each line individually
        var splitMessages = progressEvent.Message.Split("\n");
        foreach (var message in splitMessages)
        {
            this.messageQueue.Enqueue(message);
        }

        while (this.messageQueue.Count >= MessageDisplayViewModel.MaxQueueSize)
        {
            this.messageQueue.Dequeue();
        }

        this.UpdateText();
    }

    private void UpdateText()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var message in this.messageQueue)
        {
            sb.AppendLine(message);
        }

        this.Messages = sb.ToString();
        this.OnPropertyChanged(nameof(this.Messages));
    }
}