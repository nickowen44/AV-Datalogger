using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Dashboard.Models;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Dashboard;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Setup logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/dashboard.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Starting Dashboard");

            var locator = new ViewLocator();
            DataTemplates.Add(locator);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                // Setup our dependency injection
                var services = DependencyInjection.ConfigureServices();

                // Initialise our data store so a serial connection is established
                // TODO: Reassess this once we have the proper connection page in place, and initialise from there
                services.GetService<IDataStore>();

                desktop.MainWindow = new MainWindowView
                {
                    DataContext = new MainWindowViewModel(services)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }

    }
}