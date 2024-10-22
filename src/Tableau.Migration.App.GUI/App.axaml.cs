// <copyright file="App.axaml.cs" company="Salesforce, Inc.">
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

namespace Tableau.Migration.App.GUI;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using Tableau.Migration.App.Core;
using Tableau.Migration.App.Core.Hooks.Mappings;
using Tableau.Migration.App.Core.Interfaces;
using Tableau.Migration.App.GUI.Models;
using Tableau.Migration.App.GUI.Services.Implementations;
using Tableau.Migration.App.GUI.Services.Interfaces;
using Tableau.Migration.App.GUI.ViewModels;
using Tableau.Migration.App.GUI.Views;

/// <summary>
/// Main application definition.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public static IServiceProvider? ServiceProvider { get; private set; }

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
        App.ServiceProvider = serviceCollection.BuildServiceProvider();

        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Set MainWindow and DataContext via DI
            desktop.MainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();

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
        services.AddSingleton<IProgressMessagePublisher, ProgressMessagePublisher>();
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