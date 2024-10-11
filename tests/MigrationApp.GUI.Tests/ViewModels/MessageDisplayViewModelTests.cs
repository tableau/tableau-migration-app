// <copyright file="MessageDisplayViewModelTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.Tests.ViewModels
{
    using MigrationApp.Core.Entities;
    using MigrationApp.Core.Interfaces;
    using MigrationApp.GUI.ViewModels;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    public class MessageDisplayViewModelTests
    {
        private readonly Mock<IProgressMessagePublisher> publisherMock;
        private readonly ITestOutputHelper output; // Output helper for logging

        public MessageDisplayViewModelTests(ITestOutputHelper output)
        {
            this.publisherMock = new Mock<IProgressMessagePublisher>();
            this.output = output;
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            var viewModel = new MessageDisplayViewModel(this.publisherMock.Object);

            Assert.NotNull(viewModel);

            // Verify initial message.
            Assert.Equal("Migration output will be displayed here.", viewModel.Messages);

            // Verify that we can register a callback to the event without issue
            this.publisherMock.VerifyAdd(p => p.OnProgressMessage += It.IsAny<Action<ProgressEventArgs>>(), Times.Once);
        }

        [Fact]
        public void HandleProgressMessage_ShouldUpdateMessages()
        {
            var progressEventArgs = new ProgressEventArgs("ActionName", "ActionMessage");
            Action<ProgressEventArgs>? onProgressMessage = null;

            // Capture the event handler subscription
            this.publisherMock
                .SetupAdd(p => p.OnProgressMessage += It.IsAny<Action<ProgressEventArgs>>())
                .Callback<Action<ProgressEventArgs>>(handler => onProgressMessage = handler);

            var viewModel = new MessageDisplayViewModel(this.publisherMock.Object);

            // Manually invoke to trigger the event handling
            onProgressMessage?.Invoke(progressEventArgs);

            // Verify that the message was added
            var expectedMessage = "ActionName" + Environment.NewLine + "ActionMessage" + Environment.NewLine;
            Assert.Equal(expectedMessage, viewModel.Messages);
        }

        [Fact]
        public void HandleProgressMessage_ShouldRespectMaxQueueSize()
        {
            Action<ProgressEventArgs>? onProgressMessage = null;

            // Capture the event handler subscription
            this.publisherMock
                .SetupAdd(p => p.OnProgressMessage += It.IsAny<Action<ProgressEventArgs>>())
                .Callback<Action<ProgressEventArgs>>(handler => onProgressMessage = handler);

            var viewModel = new MessageDisplayViewModel(this.publisherMock.Object);

            // Fill the queue beyond its max size
            for (int i = 0; i < 2000; i++)
            {
                var progressEventArgs = new ProgressEventArgs($"Action{i}", $"Message{i}");
                onProgressMessage?.Invoke(progressEventArgs);
            }

            // Split the message up by lines
            var messages = viewModel.Messages.Split("\n");

            // Number of messages should be trimmed to the limit
            Assert.Equal(MessageDisplayViewModel.MaxQueueSize, messages.Length);
        }

        [Fact]
        public void HandleProgressMessage_EmptyProgressMessage_ShouldSkip()
        {
            var progressEventArgs = new ProgressEventArgs(string.Empty, string.Empty);
            Action<ProgressEventArgs>? onProgressMessage = null;

            // Capture the event handler subscription
            this.publisherMock
                .SetupAdd(p => p.OnProgressMessage += It.IsAny<Action<ProgressEventArgs>>())
                .Callback<Action<ProgressEventArgs>>(handler => onProgressMessage = handler);

            var viewModel = new MessageDisplayViewModel(this.publisherMock.Object);

            onProgressMessage?.Invoke(progressEventArgs);

            // Verify that the event message with no action is skipped.
            var expectedMessage = "Migration output will be displayed here.";
            Assert.Equal(expectedMessage, viewModel.Messages);
        }
    }
}