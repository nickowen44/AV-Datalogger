using System;

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
    ///     Handles actually starting the connection of portName.
    /// </summary>
    bool startConnection(string portName);

    void disconnect();

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