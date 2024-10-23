// <copyright file="AppTests.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI.Tests
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