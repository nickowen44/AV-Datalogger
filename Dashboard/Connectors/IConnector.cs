using System;
using Dashboard.Models;

namespace Dashboard.Connectors;

public interface IConnector
{
    event EventHandler<GpsData> GpsDataUpdated;
    event EventHandler<AvData> AvDataUpdated;
    event EventHandler<ResData> ResDataUpdated;
    event EventHandler<RawData> RawDataUpdated;
    event EventHandler<bool> HeartBeatUpdated;

    /// <summary>
    ///     Handles setting up the connector from the data source.
    /// </summary>
    /// <param name="args">The arguments to pass to the connector</param>
    void Start(IConnectorArgs args);


    /// <summary>
    ///     Handles stopping the connector from the data source.
    /// </summary>
    void Stop();
}