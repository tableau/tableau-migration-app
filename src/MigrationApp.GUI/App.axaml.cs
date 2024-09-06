using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml;
using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MigrationApp.Core;
using MigrationApp.GUI.ViewModels;
using MigrationApp.GUI.Views;
using System;

namespace MigrationApp.GUI;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Set MainWindow and DataContext via DI
            desktop.MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
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

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>(provider =>
        {
            return new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            };
        });
    }
}
