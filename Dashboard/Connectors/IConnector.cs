using System;
using Dashboard.Models;

namespace Dashboard.Connectors;

public interface IConnector
{
    event EventHandler<GpsData> GpsDataUpdated;
    event EventHandler<AvData> AvDataUpdated;
    event EventHandler<ResData> ResDataUpdated;
    event EventHandler<RawData> RawDataUpdated;

    /// <summary>
    ///     Handles setting up the connector to the data source.
    /// </summary>
    void Start();

    /// <summary>
    ///     Handles stopping the connector from the data source.
    /// </summary>
    void Stop();
}