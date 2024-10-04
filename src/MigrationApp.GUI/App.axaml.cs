// <copyright file="App.axaml.cs" company="Salesforce, inc">
// Copyright (c) Salesforce, inc. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MigrationApp.Core;
using MigrationApp.Core.Hooks.Mappings;
using MigrationApp.Core.Interfaces;
using MigrationApp.GUI.Models;
using MigrationApp.GUI.Services.Implementations;
using MigrationApp.GUI.Services.Interfaces;
using MigrationApp.GUI.ViewModels;
using MigrationApp.GUI.Views;
using Serilog;
using System;
using System.Collections.Generic;

/// <summary>
/// Main application definition.
/// </summary>
public partial class App : Application
{
    private IServiceProvider? serviceProvider;

    /// <summary>
    /// Initializes the app.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Callback to trigger in Avalonia framework initialization.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        this.ConfigureServices(serviceCollection);
        this.serviceProvider = serviceCollection.BuildServiceProvider();

        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Set MainWindow and DataContext via DI
            desktop.MainWindow = this.serviceProvider.GetRequiredService<MainWindow>();

            desktop.Exit += (sender, args) =>
            {
                this.OnApplicationExit();
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                this.HandleUnhandledException((Exception)args.ExceptionObject);
            };

            Console.CancelKeyPress += (sender, e) =>
            {
                this.HandleCancelKeyPress(e);
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Handles unhandled exceptions.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    public virtual void HandleUnhandledException(Exception exception)
    {
        Log.Error(exception, "Unhandled exception occurred.");
        this.OnApplicationExit();
    }

    /// <summary>
    /// Method that handles the CancelKeyPress event.
    /// </summary>
    /// <param name="e">The event arguments containing the cancellation details.</param>
    public void HandleCancelKeyPress(ConsoleCancelEventArgs e)
    {
        e.Cancel = true;  // Cancel the termination
        this.OnApplicationExit();  // Call the method that cleans up or exits the application
    }

    /// <summary>
    /// Ensures logs are flushed properly when exiting the application.
    /// </summary>
    public virtual void OnApplicationExit()
    {
        Log.CloseAndFlush();
    }

    /// <summary>
    /// Configures services for the application, including logging, settings, and dependency injection for key components.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                "Logs/migration-app.log",
                fileSizeLimitBytes: 20 * 1024 * 1024, // 20 MB file size limit
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 10,
                shared: true)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });

        IConfiguration configuration = ServiceCollectionExtensions.BuildConfiguration();
        services.AddMigrationAppCore(configuration);

        services.Configure<EmailDomainMappingOptions>(options =>
        {
            options.EmailDomain = string.Empty;
        });
        services.Configure<DictionaryUserMappingOptions>(options =>
        {
            options.UserMappings = new Dictionary<string, string>();
        });

        services.AddSingleton<IProgressUpdater, ProgressUpdater>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>(provider =>
        {
            return new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>(),
            };
        });
        services.AddTransient<IWindowProvider, WindowProvider>();
        services.AddTransient<IFilePicker, FilePicker>();
        services.AddTransient<ICsvParser, CsvHelperParser>();
    }
}