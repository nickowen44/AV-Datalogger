using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Dashboard.Models;
using Dashboard.Utils;

namespace Dashboard.Connectors.Serial;

public class SerialConnector : IConnector
{
    public event EventHandler<GpsData>? GpsDataUpdated;
    public event EventHandler<AvData>? AvDataUpdated;
    public event EventHandler<ResData>? ResDataUpdated;

    // TODO: Make the port and baud rate configurable
    // Maybe consider adding to constructor
    private static readonly SerialPort ComPort = new("COM22", 115200);

    private bool _shouldStop;

    // TODO: Handle connection status
    private DateTime _lastMessageReceived = DateTime.Now;

    // TODO: Consider making this configurable
    // Connection timeout in seconds
    private const double ConnectionTimeout = 5;

    public void Start()
    {
        ComPort.Open();

        // Start a new thread to read the data from the serial port
        new Thread(() =>
        {
            while (!_shouldStop)
            {
                var data = ComPort.ReadLine();

                ParseMessage(data);

                _lastMessageReceived = DateTime.Now;
            }
        }).Start();
    }

    public void Stop()
    {
        _shouldStop = true;

        ComPort.Close();
    }

    private void ParseMessage(string message)
    {
        // All messages start with the format "#ID=<ID>|UTC=<TIME>|<MSG>", so we split the message by the '|', and remove the first 2 elements
        var split = message.Split('|')[2..];

        // We have 3 message types: GPS NVP, AV Status, and RES Message
        // GPS starts with "LAT", AV Status starts with "SA", and RES starts with "RES"
        // First we parse our message into K:V pairs, then we identify the message type
        var values = split.Select(s => s.Split('=')).ToDictionary(s => s[0], s => s[1]);

        if (values.ContainsKey("LAT"))
            ParseGpsMessage(values);
        else if (values.ContainsKey("SA"))
            ParseAvStatusMessage(values);
        else if (values.ContainsKey("RES")) ParseResMessage(values);
        else Console.WriteLine($"Error: Unable to parse serial message {message}");
    }

    private void ParseGpsMessage(Dictionary<string, string> values)
    {
        // TODO: Error handling possibly...
        GpsDataUpdated?.Invoke(this, new GpsData
        {
            Latitude = ParseDouble(values["LAT"]),
            Longitude = ParseDouble(values["LNG"]),
            AltitudeMeters = ParseDouble(values["ALM"]),
            AltitudeKilmeters = ParseDouble(values["AKM"]),
            MeterPerSecond = ParseDouble(values["MPS"]),
            KilometersPerHour = ParseDouble(values["KMH"]),
            HdopFixAge = ParseInt(values["HFA"]),
            Hdop = ParseDouble(values["HDOP"]),
            HVal = ParseInt(values["HVAL"]),
            SatFixAge = ParseInt(values["SFA"]),
            SatCount = ParseInt(values["NOS"]),
            SpeedFixAge = ParseInt(values["SPFA"]),
            AltFixAge = ParseInt(values["AFA"])
        });
    }

    private void ParseAvStatusMessage(Dictionary<string, string> values)
    {
        AvDataUpdated?.Invoke(this, new AvData
        {
            Speed = new ValuePair<double>
                { Actual = ParseDouble(values["SA"]), Target = ParseDouble(values["ST"]) },
            SteeringAngle = new ValuePair<double>
                { Actual = ParseDouble(values["STA"]), Target = ParseDouble(values["STT"]) },
            BrakeActuation = new ValuePair<double>
                { Actual = ParseDouble(values["BRA"]), Target = ParseDouble(values["BRT"]) },
            MotorMoment = new ValuePair<double>
                { Actual = ParseDouble(values["MMA"]), Target = ParseDouble(values["MMT"]) },
            LateralAcceleration = ParseDouble(values["ALAT"]),
            LongitudinalAcceleration = ParseDouble(values["ALON"]),
            YawRate = ParseDouble(values["YAW"]),
            AutonomousSystemState = ParseInt(values["AST"]),
            EmergencyBrakeState = ParseInt(values["EBS"]),
            MissionIndicator = ParseInt(values["AMI"]),
            SteeringState = ParseBool(values["STS"]),
            ServiceBrakeState = ParseBool(values["SBS"]),
            LapCount = ParseInt(values["LAP"]),
            ConeCountPerLap = ParseInt(values["CCA"]),
            ConeCountTotal = ParseInt(values["CCT"])
        });
    }

    private void ParseResMessage(Dictionary<string, string> values)
    {
        ResDataUpdated?.Invoke(this, new ResData
        {
            ResState = ParseBool(values["RES"]),
            K2State = ParseBool(values["K2T"]),
            K3State = ParseBool(values["K3B"]),
            ResRadioQuality = ParseInt(values["RRQ"]),
            ResNodeId = ParseInt(values["NID"])
        });
    }

    // TODO: Remove below when we have the actual values
    private readonly Random _random = new();

    private double ParseDouble(string value)
    {
        return value.StartsWith('#') ? _random.NextDouble() * 10 : double.Parse(value);
    }

    private int ParseInt(string value)
    {
        return value.StartsWith('#') ? _random.Next() : int.Parse(value);
    }

    private bool ParseBool(string value)
    {
        return value.StartsWith('#') ? _random.Next() % 2 == 0 : value == "1";
    }

    private bool IsConnected()
    {
        return (DateTime.Now - _lastMessageReceived).TotalSeconds < ConnectionTimeout;
    }
}