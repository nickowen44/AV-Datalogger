using System;

namespace Dashboard.Models;

public interface IDataStore
{
    event EventHandler DataUpdated;
    void UpdateData(string autonomousMissionIndicator, double speedActual, double speedTarget, double steeringAngleActual, double steeringAngleTarget, double brakePressureActual, double brakePressureTarget, bool remoteEmergencyStopStatus);
    void OnDataUpdated(object? sender, EventArgs e);

    string AutonomousMissionIndicator { get; }
    double SpeedActual { get; }
    double SpeedTarget { get; }
    double SteeringAngleActual { get; }
    double SteeringAngleTarget { get; }
    double BrakePressureActual { get; }
    double BrakePressureTarget { get; }
    bool RemoteEmergencyStopStatus { get; }

    /// <summary>
    ///     Handles the cleanup when the data store is no longer needed.
    ///     Such as removing event handlers and stopping the store's connector.
    /// </summary>
    void Dispose();
}