using System;

namespace Dashboard.Models;

public interface IDataStore
{
    GpsData? GpsData { get; }
    AvData? AvStatusData { get; }
    ResData? ResData { get; }
    event EventHandler GpsDataUpdated;
    event EventHandler AvDataUpdated;
    event EventHandler ResDataUpdated;

    /// <summary>
    ///     Handles the cleanup when the data store is no longer needed.
    ///     Such as removing event handlers and stopping the store's connector.
    /// </summary>
    void Dispose();
}