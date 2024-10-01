using System;
using Dashboard.Connectors;
using Dashboard.Connectors.Serial;
using Dashboard.Models;
using Dashboard.ViewModels;
using Dashboard.Views;
using Microsoft.Extensions.DependencyInjection;
using Tmds.DBus.Protocol;

namespace Dashboard.Utils;

public static class DependencyInjection
{
    /// <summary>
    ///     Configures the services for the application.
    /// </summary>
    /// <returns>A configured <see cref="IServiceProvider" />.</returns>
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDataStore, DataStore>();
        services.AddSingleton<IConnector, SerialConnector>();

        services.AddTransient<DataViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ISerialPort, SerialPortWrapper>();
        services.AddTransient<ScrutineeringViewModel>();
        services.AddTransient<FooterViewModel>();
        services.AddTransient<AboutViewModel>();
        services.AddTransient<ConnectionViewModel>();

        return services.BuildServiceProvider();
    }
}