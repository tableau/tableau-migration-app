using System;
using Xunit;
using Avalonia.Controls.ApplicationLifetimes;

namespace MigrationApp.GUI.Tests
{
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
            var cancelEventArgs = CreateConsoleCancelEventArgs(ConsoleSpecialKey.ControlC);

            app.HandleCancelKeyPress(cancelEventArgs);

            Assert.True(app.ExitCalled, "OnApplicationExit should be called when CancelKeyPress is handled.");
            Assert.True(cancelEventArgs.Cancel, "Cancel should be set to true when handling CancelKeyPress.");
        }

        private ConsoleCancelEventArgs CreateConsoleCancelEventArgs(ConsoleSpecialKey key)
        {
            var consoleCancelEventArgsType = typeof(ConsoleCancelEventArgs);
            var ctor = consoleCancelEventArgsType.GetConstructor(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null, new[] { typeof(ConsoleSpecialKey) }, null);

            return (ConsoleCancelEventArgs)ctor.Invoke(new object[] { key });
        }
    }

    public class MockClassicDesktopStyleApplicationLifetime
    {
        private readonly IClassicDesktopStyleApplicationLifetime _desktopLifetime;

        public MockClassicDesktopStyleApplicationLifetime(TestableApp app)
        {
            _desktopLifetime = new ClassicDesktopStyleApplicationLifetime();
            _desktopLifetime.Exit += (sender, args) => app.OnApplicationExit();
        }

        public void TriggerExit()
        {
            _desktopLifetime.Shutdown();
        }
    }

    public class TestableApp : App
    {
        public bool ExitCalled { get; private set; } = false;

        public override void OnApplicationExit()
        {
            ExitCalled = true;
            base.OnApplicationExit();
        }
    }
}
