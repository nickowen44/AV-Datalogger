using System;

namespace Dashboard.Connectors;

public interface IConnector
{
    event EventHandler<DataUpdatedEventArgs> DataUpdated;

    /// <summary>
    ///     Handles setting up the connector to the data source.
    /// </summary>
    void Start();

    /// <summary>
    ///     Handles stopping the connector from the data source.
    /// </summary>
    void Stop();
}