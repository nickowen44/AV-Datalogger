using Dashboard.Utils;

namespace Dashboard.Models;

public record AvData
{
    

    public ValuePair<double> Speed { get; init; } = new()
    {
        Actual = 0, Target = 0
    };

    public ValuePair<double> SteeringAngle { get; init; } = new()
    {
        Actual = 0, Target = 0
    };

    public ValuePair<double> BrakeActuation { get; init; } = new()
    {
        Actual = 0, Target = 0
    };

    public ValuePair<double> MotorMoment { get; init; } = new()
    {
        Actual = 0, Target = 0
    };
    public double LateralAcceleration { get; init; }
    public double LongitudinalAcceleration { get; init; }
    public double YawRate { get; init; }
    public int AutonomousSystemState { get; init; }
    public int EmergencyBrakeState { get; init; }
    public int MissionIndicator { get; init; }
    public bool SteeringState { get; init; }
    public bool ServiceBrakeState { get; init; }
    public int LapCount { get; init; }
    public int ConeCountPerLap { get; init; }
    public int ConeCountTotal { get; init; }
    public string CarId { get; init; }
    public string UTCTime { get; init; }
}