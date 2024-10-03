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
    ///     Handles setting up the connector when the DataStore.
    /// </summary>
    /// <param name="portName">The name of the port to connect to. Defaults to COM22</param>

    void Start(string portName = "COM22");


    /// <summary>
    ///     Handles stopping the connector from the data source.
    /// </summary>
    void Stop();
}