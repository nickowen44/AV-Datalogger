using Dashboard.Connectors.Dummy;

namespace Dashboard.Connectors;

public class NullConnectorFactory : IConnectorFactory
{
    public IConnector CreateConnector(IConnectorArgs args)
    {
        return new DummyConnector();
    }
}