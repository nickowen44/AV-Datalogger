using System;
using System.Threading;
using Dashboard.Models;
using Dashboard.Utils;

namespace Dashboard.Connectors;

public class DummyConnector : IConnector
{
    public event EventHandler<GpsData>? GpsDataUpdated;
    public event EventHandler<AvData>? AvDataUpdated;
    public event EventHandler<ResData>? ResDataUpdated;
    public event EventHandler<RawData>? RawDataUpdated;
    public event EventHandler<bool>? HeartBeatUpdated;

    private bool _shouldStop;
    /// <summary>
    ///     Handles setting up the connector to the data source when no port name is passed, Defaults to COM22.
    /// </summary>
    public void Start()
    {
        var random = new Random();

        new Thread(() =>
        {
            while (!_shouldStop)
            {
                // Generate random GpsData
                var gpsData = new GpsData
                {
                    Latitude = random.NextDouble() * 180 - 90,
                    Longitude = random.NextDouble() * 360 - 180,
                    AltitudeMeters = random.NextDouble() * 10000,
                    AltitudeKilmeters = random.NextDouble() * 10,
                    MeterPerSecond = random.NextDouble() * 100,
                    KilometersPerHour = random.NextDouble() * 360,
                    HdopFixAge = random.Next(100),
                    Hdop = random.NextDouble() * 50,
                    HVal = random.Next(100),
                    SatFixAge = random.Next(100),
                    SatCount = random.Next(50),
                    SpeedFixAge = random.Next(100),
                    AltFixAge = random.Next(100)
                };
                GpsDataUpdated?.Invoke(this, gpsData);

                // Generate random AvData
                var avData = new AvData
                {
                    Speed = new ValuePair<double>
                    {
                        Actual = random.NextDouble() * 100, Target = random.NextDouble() * 100
                    },
                    SteeringAngle = new ValuePair<double>
                    {
                        Actual = random.NextDouble() * 360 - 180, Target = random.NextDouble() * 360 - 180
                    },
                    BrakeActuation = new ValuePair<double>
                    {
                        Actual = random.NextDouble() * 100, Target = random.NextDouble() * 100
                    },
                    MotorMoment = new ValuePair<double>
                    {
                        Actual = random.NextDouble() * 1000, Target = random.NextDouble() * 1000
                    },
                    LateralAcceleration = random.NextDouble() * 10,
                    LongitudinalAcceleration = random.NextDouble() * 10,
                    YawRate = random.NextDouble() * 360,
                    AutonomousSystemState = random.Next(10),
                    EmergencyBrakeState = random.Next(2),
                    MissionIndicator = random.Next(10),
                    SteeringState = random.Next(2) == 0,
                    ServiceBrakeState = random.Next(2) == 0,
                    LapCount = random.Next(100),
                    ConeCountPerLap = random.Next(100),
                    ConeCountTotal = random.Next(1000)
                };
                AvDataUpdated?.Invoke(this, avData);

                // Generate random ResData
                var resData = new ResData
                {
                    ResState = random.Next(2) == 0,
                    K2State = random.Next(2) == 0,
                    K3State = random.Next(2) == 0,
                    ResRadioQuality = random.Next(100),
                    ResNodeId = random.Next(1000)
                };
                ResDataUpdated?.Invoke(this, resData);

                Thread.Sleep(1000);
            }
        }).Start();
    }
    public void Start(string portName)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        _shouldStop = true;
    }
}