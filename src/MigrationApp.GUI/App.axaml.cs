// <copyright file="App.axaml.cs" company="Salesforce, inc.">
// Copyright (c) Salesforce, inc.. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MigrationApp.GUI;

using Avalonia;
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
using MigrationApp.GUI.ViewModels;
using MigrationApp.GUI.Views;
using System;

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
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure =>
        {
            configure.AddConsole();

            // Add other logging providers
        });
        IConfiguration configuration = ServiceCollectionExtensions.BuildConfiguration();
        services.AddMigrationAppCore(configuration);

        services.Configure<EmailDomainMappingOptions>(options =>
        {
            options.EmailDomain = string.Empty;
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
    }
}