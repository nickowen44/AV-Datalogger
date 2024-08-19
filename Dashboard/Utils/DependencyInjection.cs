using System;
using Dashboard.Connectors;
using Dashboard.Models;
using Dashboard.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Utils;

public static class DependencyInjection
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDataStore, Models.DataStore>();
        services.AddSingleton<IConnector, DummyConnector>();

        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }
}