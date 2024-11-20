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

namespace MigrationApp.Tests.ViewModels;

using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.App.Core.Entities;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;
using Xunit;
using Xunit.Abstractions;

public class MessageDisplayViewModelTests
{
    private readonly Mock<IProgressMessagePublisher> publisherMock;
    private readonly ITestOutputHelper output; // Output helper for logging
    private readonly Mock<ILogger<MessageDisplayViewModel>> loggerMock;
    private readonly Mock<IMigrationTimer> migrationTimerMock;
    private readonly MessageDisplayViewModel viewModel;
    private Action<ProgressEventArgs>? onProgressMessage = null;

    public MessageDisplayViewModelTests(ITestOutputHelper output)
    {
        this.publisherMock = new Mock<IProgressMessagePublisher>();

        // Capture the event handler during mock setup
        this.publisherMock
            .SetupAdd(p => p.OnProgressMessage += It.IsAny<Action<ProgressEventArgs>>())
            .Callback<Action<ProgressEventArgs>>(handler => this.onProgressMessage = handler);

        this.loggerMock = new Mock<ILogger<MessageDisplayViewModel>>();
        this.migrationTimerMock = new Mock<IMigrationTimer>();
        this.viewModel = new MessageDisplayViewModel(
            this.migrationTimerMock.Object,
            this.publisherMock.Object,
            this.loggerMock.Object);
        this.output = output;
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        Assert.NotNull(this.viewModel);

        // Verify initial message.
        Assert.Equal("Migration output will be displayed here.", this.viewModel.Messages);

        // Verify that we can register a callback to the event without issue
        this.publisherMock.VerifyAdd(p => p.OnProgressMessage += It.IsAny<Action<ProgressEventArgs>>(), Times.Once);
    }

    [Fact]
    public void HandleProgressMessage_ShouldUpdateMessages()
    {
        // Ensure the event handler is properly assigned
        Assert.NotNull(this.onProgressMessage);

        var progressEventArgs = new ProgressEventArgs("ActionMessage");

        // Manually invoke the captured event handler to simulate an event
        this.onProgressMessage?.Invoke(progressEventArgs);

        // Verify that the message was added
        var expectedMessage = "ActionMessage" + Environment.NewLine;
        Assert.Equal(expectedMessage, this.viewModel.Messages);
    }

    [Fact]
    public void HandleProgressMessage_ShouldRespectMaxQueueSize()
    {
        // Ensure the event handler is properly assigned
        Assert.NotNull(this.onProgressMessage);

        // Fill the queue beyond its max size
        for (int i = 0; i < 2000; i++)
        {
            var progressEventArgs = new ProgressEventArgs($"Message{i}");
            this.onProgressMessage?.Invoke(progressEventArgs);
        }

        // Split the message up by lines
        var messages = this.viewModel.Messages.Split('\n');

        // Number of messages should be trimmed to the limit
        Assert.Equal(MessageDisplayViewModel.MaxQueueSize, messages.Length);
    }
}