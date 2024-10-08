using System;
using Dashboard.Connectors;

namespace Dashboard.Models;

public interface IDataStore
{
    event EventHandler GpsDataUpdated;
    event EventHandler AvDataUpdated;
    event EventHandler ResDataUpdated;
    event EventHandler RawDataUpdated;
    event EventHandler<bool> HeartBeatUpdated;
    event EventHandler<string> ConsoleMessageUpdated;
    GpsData? GpsData { get; }
    AvData? AvStatusData { get; }
    ResData? ResData { get; }
    RawData? RawData { get; }
    bool? HeartBeat { get; }
    
    /// <summary>
    ///     Handles setting up the data ingress connection to the data source.
    ///     Dynamically creates the connector based on the type of the connector arguments.
    /// </summary>
    bool Connect(IConnectorArgs args);

    void Disconnect();

    /// <summary>
    ///     Handles the cleanup when the data store is no longer needed.
    ///     Such as removing event handlers and stopping the store's connector.
    /// </summary>
    void Dispose();

    /// <summary>
    ///     The log buffer is used to store log messages until the UI is ready to display them.
    ///     This function flushes the log buffer and sends the messages via events.
    /// </summary>
    void FlushLogBuffer();
}