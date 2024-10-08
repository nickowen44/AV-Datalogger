using System;
using Dashboard.Connectors;
using Dashboard.Connectors.Csv;
using Dashboard.Connectors.Serial;
using Dashboard.Models;
using Dashboard.Serialisation;
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

        services.AddLogging(builder => builder.AddLogger());

        services.AddSingleton<IDataStore, DataStore>();
        services.AddSingleton<IYamlLoader, YamlLoader>();

        services.AddTransient<DataViewModel>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ISerialPort, SerialPortWrapper>();
        services.AddTransient<ScrutineeringViewModel>();
        services.AddTransient<FooterViewModel>();
        services.AddTransient<AboutViewModel>();
        services.AddTransient<ConnectionViewModel>();
        services.AddTransient<IDataSerialisationFactory, DataSerialisationFactory>();

        services.AddTransient<IConnectorFactory, ConnectorFactory>();
        services.AddTransient<CsvConnector>();
        services.AddTransient<SerialConnector>();

        return services.BuildServiceProvider();
    }
}