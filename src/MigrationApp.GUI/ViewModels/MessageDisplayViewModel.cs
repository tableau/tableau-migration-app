// <copyright file="MessageDisplayViewModel.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MigrationApp.Core.Entities;
using MigrationApp.Core.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

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
    public MessageDisplayViewModel(IProgressMessagePublisher publisher)
    {
        this.Messages = "Migration output will be displayed here.";
        this.publisher = publisher;
        this.messageQueue = new Queue<string>();
        this.publisher.OnProgressMessage += this.HandleProgressMessage;
        this.logger = App.ServiceProvider?.GetRequiredService<ILogger<MessageDisplayViewModel>>();
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
        // Skip the message if no associated action is present
        if (progressEvent.Action == string.Empty)
        {
            return;
        }

        this.messageQueue.Enqueue($"{progressEvent.Action}");

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
    }
}