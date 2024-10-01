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
    ///     Handles setting up the connector when the DataStore is passed a port name.
    /// </summary>
    void Start(string portName);
    
    /// <summary>
    ///     Handles setting up the connector to the data source when no port name is passed, Defaults to COM22.
    /// </summary>
    void Start();

    /// <summary>
    ///     Handles stopping the connector from the data source.
    /// </summary>
    void Stop();
}