using System;

namespace Dashboard.Models;

public class DataStore : IDataStore
{
    public event EventHandler? DataUpdated;

    public double Speed { get; private set; }
    public double SteeringAngle { get; private set; }
    public double BrakePressure { get; private set; }

    public void UpdateData(double speed, double steeringAngle, double brakePressure)
    {
        Speed = speed;
        SteeringAngle = steeringAngle;
        BrakePressure = brakePressure;

        DataUpdated?.Invoke(this, EventArgs.Empty);
    }
}