// <copyright file="AppTests.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI.Tests
{
    using System;
    using Avalonia.Controls.ApplicationLifetimes;
    using Xunit;

    public class AppTests
    {
        [Fact]
        public void Should_Call_OnApplicationExit_When_Desktop_Exit_Is_Triggered()
        {
            var app = new TestableApp();
            var mockDesktopLifetime = new MockClassicDesktopStyleApplicationLifetime(app);

            mockDesktopLifetime.TriggerExit();

            Assert.True(app.ExitCalled, "OnApplicationExit should be called once during Exit.");
        }

        [Fact]
        public void Should_Call_OnApplicationExit_When_UnhandledExceptionOccurs()
        {
            var app = new TestableApp();
            var exception = new Exception("Test exception");

            app.HandleUnhandledException(exception);

            Assert.True(app.ExitCalled, "OnApplicationExit should be called when an unhandled exception occurs.");
        }

        [Fact]
        public void Should_Call_OnApplicationExit_When_CancelKeyPress_Is_Handled()
        {
            var app = new TestableApp();
            var cancelEventArgs = this.CreateConsoleCancelEventArgs(ConsoleSpecialKey.ControlC);

            app.HandleCancelKeyPress(cancelEventArgs);

            Assert.True(app.ExitCalled, "OnApplicationExit should be called when CancelKeyPress is handled.");
            Assert.True(cancelEventArgs.Cancel, "Cancel should be set to true when handling CancelKeyPress.");
        }

        private ConsoleCancelEventArgs CreateConsoleCancelEventArgs(ConsoleSpecialKey key)
        {
            var consoleCancelEventArgsType = typeof(ConsoleCancelEventArgs);
            var ctor = consoleCancelEventArgsType.GetConstructor(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                new[] { typeof(ConsoleSpecialKey) },
                null);

            if (ctor == null)
            {
                throw new InvalidOperationException("Could not retrieve ConsoleCancelEventArgs constructor.");
            }

            return (ConsoleCancelEventArgs)ctor.Invoke(new object[] { key });
        }

        private class MockClassicDesktopStyleApplicationLifetime
        {
            private readonly IClassicDesktopStyleApplicationLifetime desktopLifetime;

            public MockClassicDesktopStyleApplicationLifetime(TestableApp app)
            {
                this.desktopLifetime = new ClassicDesktopStyleApplicationLifetime();
                this.desktopLifetime.Exit += (sender, args) => app.OnApplicationExit();
            }

            public void TriggerExit()
            {
                this.desktopLifetime.Shutdown();
            }
        }

        private class TestableApp : App
        {
            public bool ExitCalled { get; private set; } = false;

            public override void OnApplicationExit()
            {
                this.ExitCalled = true;
                base.OnApplicationExit();
            }
        }
    }
}