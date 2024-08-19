using System;

namespace Dashboard.Models;

public interface IDataStore
{
    event EventHandler DataUpdated;

    double Speed { get; }
    double SteeringAngle { get; }
    double BrakePressure { get; }

    /// <summary>
    ///     Updates the data store with the new values.
    /// </summary>
    /// <param name="speed">The car's speed</param>
    /// <param name="steeringAngle">The car's steering angle</param>
    /// <param name="brakePressure">The car's brake pressure</param>
    void UpdateData(double speed, double steeringAngle, double brakePressure);
}