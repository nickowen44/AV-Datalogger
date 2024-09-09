using System;
using Dashboard.Connectors;
using Dashboard.Connectors.Serial;
using Dashboard.Models;
using Dashboard.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddTransient<MainViewModel>();
        services.AddTransient<ISerialPort, SerialPortWrapper>();

        return services.BuildServiceProvider();
    }
}