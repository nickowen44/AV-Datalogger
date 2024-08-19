using System;

namespace Dashboard.Connectors;

public class DataUpdatedEventArgs(double speed, double steeringAngle, double brakePressure) : EventArgs
{
    public double Speed { get; } = speed;
    public double SteeringAngle { get; } = steeringAngle;
    public double BrakePressure { get; } = brakePressure;
}