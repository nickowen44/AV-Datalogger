namespace Dashboard.Models;

public record GpsData
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double AltitudeMeters { get; init; }
    public double AltitudeKilmeters { get; init; }
    public double MeterPerSecond { get; init; }
    public double KilometersPerHour { get; init; }
    public int HdopFixAge { get; init; }
    public double Hdop { get; init; }
    public int HVal { get; init; }
    public int SatFixAge { get; init; }
    public int SatCount { get; init; }
    public int SpeedFixAge { get; init; }
    public int AltFixAge { get; init; }
}