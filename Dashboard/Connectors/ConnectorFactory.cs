using System;
using Dashboard.Connectors.Csv;
using Dashboard.Connectors.Dummy;
using Dashboard.Connectors.Serial;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Connectors;

public class ConnectorFactory(IServiceProvider serviceProvider) : IConnectorFactory
{
    public IConnector CreateConnector(IConnectorArgs args)
    {
        return args switch
        {
            CsvConnectorArgs => serviceProvider.GetRequiredService<CsvConnector>(),
            DummyConnectorArgs => serviceProvider.GetRequiredService<DummyConnector>(),
            SerialConnectorArgs => serviceProvider.GetRequiredService<SerialConnector>(),
            _ => throw new ArgumentException("Invalid connector type", nameof(args))
        };
    }
}