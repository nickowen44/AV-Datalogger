using System;

namespace Dashboard.Connectors;

public class DataUpdatedEventArgs : EventArgs
{
    public string AutonomousMissionIndicator { get; }
    public double SpeedActual { get; }
    public double SpeedTarget { get; }
    public double SteeringAngleActual { get; }
    public double SteeringAngleTarget { get; }
    public double BrakePressureActual { get; }
    public double BrakePressureTarget { get; }
    public bool RemoteEmergencyStopStatus { get; }

    public DataUpdatedEventArgs(string autonomousMissionIndicator, double speedActual, double speedTarget, double steeringAngleActual, double steeringAngleTarget, double brakePressureActual, double brakePressureTarget, bool remoteEmergencyStopStatus)
    {
        AutonomousMissionIndicator = autonomousMissionIndicator;
        SpeedActual = speedActual;
        SpeedTarget = speedTarget;
        SteeringAngleActual = steeringAngleActual;
        SteeringAngleTarget = steeringAngleTarget;
        BrakePressureActual = brakePressureActual;
        BrakePressureTarget = brakePressureTarget;
        RemoteEmergencyStopStatus = remoteEmergencyStopStatus;
    }
}