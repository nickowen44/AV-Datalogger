﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Dashboard.Utils;
using Dashboard.ViewModels;
using Dashboard.Views;

namespace Dashboard;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var locator = new ViewLocator();
        DataTemplates.Add(locator);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Setup our dependency injection
            var services = DependencyInjection.ConfigureServices();

            desktop.MainWindow = new MainWindowView
            {
                DataContext = new MainWindowViewModel(services)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}