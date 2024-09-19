using Dashboard.Utils;

namespace Dashboard.Models;

public record StatusStore {

    public string AutonomousMissionIndicator { get; init; } = string.Empty;

    public double SpeedActual { get; init; }

    public double SpeedTarget { get; init; }

    public double SteeringAngleActual { get; init; }

    public double SteeringAngleTarget { get; init; }

    public double BrakePressureActual { get; init; }

    public double BrakePressureTarget { get; init; }

    public bool RemoteEmergencyStopStatus { get; init; }
}