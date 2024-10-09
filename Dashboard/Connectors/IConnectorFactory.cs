namespace Dashboard.Connectors;

public interface IConnectorFactory
{
    /// <summary>
    ///     Creates a connector based on the provided arguments, allowing for runtime selection of connector type.
    /// </summary>
    /// <param name="args">The connector args, <see cref="IConnectorArgs" /></param>
    IConnector CreateConnector(IConnectorArgs args);
}