using System;

namespace Dashboard.Models;

public interface IDataStore
{
    event EventHandler GpsDataUpdated;
    event EventHandler AvDataUpdated;
    event EventHandler ResDataUpdated;

    GpsData? GpsData { get; }
    AvData? AvStatusData { get; }
    ResData? ResData { get; }

    /// <summary>
    ///     Handles the cleanup when the data store is no longer needed.
    ///     Such as removing event handlers and stopping the store's connector.
    /// </summary>
    void Dispose();
}