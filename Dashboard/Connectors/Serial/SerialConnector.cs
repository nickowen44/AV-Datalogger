using System;
using System.Collections.Generic;
using Dashboard.Models;
using Dashboard.Utils;

namespace Dashboard.Connectors.Serial;

public class SerialConnector(ISerialPort comPort) : IConnector
{
    public event EventHandler<GpsData>? GpsDataUpdated;
    public event EventHandler<AvData>? AvDataUpdated;
    public event EventHandler<ResData>? ResDataUpdated;

    public void Start()
    {
        // Set up the connection to the serial port
        comPort.Configure("COM22", 115200);

        // Set up the event handler for when data is received
        comPort.DataReceived += OnDataReceived;

        // Open our serial port
        comPort.Open();
    }

    private void OnDataReceived(object? _, SerialPortData data)
    {
        // If multiple messages are received at once, we need to handle them. To do this, we read the entire buffer
        var buffer = data.Buffer;
        var messages = buffer.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        // Then we split the buffer by the carriage return delimiter, and parse each message
        foreach (var msg in messages) ParseMessage(msg);
    }

    public void Stop()
    {
        // Remove the event handler
        comPort.DataReceived -= OnDataReceived;

        // Close the serial port
        comPort.Close();
    }

    private void ParseMessage(string message)
    {
        // All messages start with the format "#ID=<ID>|UTC=<TIME>|<MSG>", so we split the message by the '|', and remove the first 2 elements
        // First we validate that we have the correct message format
        if (!message.StartsWith("#ID=") || !message.Contains("|UTC="))
            throw new InvalidOperationException("Invalid message format received", new Exception(message));
        
        var split = message.Split('|')[2..];

        // We have 3 message types: GPS NVP, AV Status, and RES Message
        // GPS starts with "LAT", AV Status starts with "SA", and RES starts with "RES"
        // First we parse our message into K:V pairs, then we identify the message type
        var values = new Dictionary<string, string>();
        foreach (var s in split)
        {
            var pair = s.Split('=');

            // If we don't have a key-value pair, we throw an exception
            if (pair.Length != 2)
                throw new InvalidOperationException($"Invalid key-value pair: {s}",
                    new Exception($"Buffer str: {message}"));

            values.Add(pair[0], pair[1]);
        }

        if (values.ContainsKey("LAT"))
            ParseGpsMessage(values);
        else if (values.ContainsKey("SA"))
            ParseAvStatusMessage(values);
        else if (values.ContainsKey("RES")) ParseResMessage(values);
        else
            throw new InvalidOperationException("Unknown message type received",
                new Exception($"Buffer str: {message}"));
    }

    private void ParseGpsMessage(Dictionary<string, string> values)
    {
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

}