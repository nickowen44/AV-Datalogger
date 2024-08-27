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
    /// Updates the Av-logger's GPS data in the data store.
    /// </summary>
    /// <param name="gpsData">The new data to store.</param>
    // void UpdateGpsData(GpsData gpsData);

    /// <summary>
    ///     Updates the AV data in the data store.
    /// </summary>
    /// <param name="avData">The AV data to store.</param>
    // void UpdateAvData(AvData avData);

    /// <summary>
    ///     Updates the RES data in the data store.
    /// </summary>
    /// <param name="resData">The RES data to store.</param>
    // void UpdateResData(ResData resData);

    /// <summary>
    ///     Handles the cleanup when the data store is no longer needed.
    ///     Such as removing event handlers and stopping the store's connector.
    /// </summary>
    void Dispose();
}