using System;

namespace Dashboard.Models;

public interface IDataStore
{
    event EventHandler GpsDataUpdated;
    event EventHandler AvDataUpdated;
    event EventHandler ResDataUpdated;
    event EventHandler RawDataUpdated;
    event EventHandler<bool> HeartBeatUpdated;
    GpsData? GpsData { get; }
    AvData? AvStatusData { get; }
    ResData? ResData { get; }
    RawData? RawData { get; }
    bool? HeartBeat { get; }
    bool startConnection(string portName);

    /// <summary>
    ///     Handles the cleanup when the data store is no longer needed.
    ///     Such as removing event handlers and stopping the store's connector.
    /// </summary>
    void Dispose();
}