using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Dashboard.Models;
using Dashboard.Utils;
using Microsoft.Extensions.Logging;

namespace Dashboard.Connectors.Serial;

public partial class SerialConnector(ISerialPort comPort, ILogger<SerialConnector> logger) : IConnector
{
    private const string HeartBeatMessage = "CON?";
    private readonly ManualResetEvent _heartbeatEvent = new(false);


    // TODO: Remove below when we have the actual values
    private readonly Random _random = new();
    private bool _heartBeatShouldRun = true;
    private Thread? _heartbeatThread;
    public event EventHandler<GpsData>? GpsDataUpdated;
    public event EventHandler<AvData>? AvDataUpdated;
    public event EventHandler<ResData>? ResDataUpdated;

    public event EventHandler<RawData>? RawDataUpdated;
    public event EventHandler<bool>? HeartBeatUpdated;

    public void Start(IConnectorArgs args)
    {
        logger.LogInformation("Starting Serial Connector");

        if (args is not SerialConnectorArgs serialArgs)
        {
            logger.LogError("Invalid arguments passed to Serial Connector");
            return;
        }

        // Set up the connection to the serial port
        comPort.Configure(serialArgs.PortName, 115200);

        // Set up the event handler for when data is received
        comPort.DataReceived += OnDataReceived;

        //Set up Heart Beat thread 
        _heartbeatThread = new Thread(() =>
        {
            while (_heartBeatShouldRun)
            {
                if (!comPort.IsConnected)
                {
                    // If Heart beat should be sent then write and wait 1 second.
                    SendHeartbeat();
                    Thread.Sleep(1000);
                }

                _heartbeatEvent.WaitOne(1000);
            }
        });
        _heartbeatThread.Start();
        _heartBeatShouldRun = true;

        // Open our serial port
        comPort.Open();

        RawDataUpdated?.Invoke(this, new RawData
        {
            CarId = "",
            UTCTime = DateTime.Now,
            ConnectionStatus = true
        });
    }

    public void Stop()
    {
        logger.LogInformation("Stopping Serial Connector");

        // Remove the event handler
        comPort.DataReceived -= OnDataReceived;

        // Stop the Heart Beat thread
        _heartBeatShouldRun = false;
        _heartbeatEvent.Set();

        // Update the Connection status.
        RawDataUpdated?.Invoke(this, new RawData
        {
            CarId = "",
            UTCTime = DateTime.Now,
            ConnectionStatus = false
        });

        // Close the serial port
        comPort.Close();
    }

    [GeneratedRegex(@"^ID=.*\|UTC=.*\|.*")]
    private static partial Regex MyRegex();

    private void OnDataReceived(object? _, SerialPortData data)
    {
        logger.LogDebug("Received data from serial port: {data}", data.Buffer);

        // We got a new message from the serial port, parse it, removing the newline / return characters
        ParseMessage(data.Buffer.Trim());
    }

    private void ParseMessage(string message)
    {
        // First we validate that we have the correct message format
        if (!MyRegex().IsMatch(message) && message != "OK")
        {
            // Skips message if the format is invalid so the thread doesn't kill itself.
            logger.LogWarning("Invalid message format received: {message}", message);
            return;
        }

        if (message == "OK")
        {
            HeartBeatUpdated?.Invoke(this, true);
            logger.LogInformation("Heartbeat acknowledged by AV Logger");
            return;
        }

        // remove ending pipe on message
        message = message.TrimEnd('|');
        var split = message.Split('|');

        // We have 3 message types: GPS NVP, AV Status, and RES Message
        // GPS starts with "LAT", AV Status starts with "SA", and RES starts with "RES"
        // First we parse our message into K:V pairs, then we identify the message type
        var values = new Dictionary<string, string>();
        foreach (var s in split)
        {
            var pair = s.Split('=');

            // If we don't have a key-value pair, we throw an exception
            if (pair.Length < 2)
            {
                logger.LogError("Invalid key-value pair: {s}, Skipping value: {message}", s, message);
                var key = s.Replace("=", "");
                values[key] = "0.0";
            }
            else
            {
                values[pair[0]] = pair[1];
            }
        }

        if (values.ContainsKey("LAT"))
            // Check UTC here, if GPS isn't initialized yet, log error

            try
            {
                ParseUTCTime(values["UTC"]);
                ParseGpsMessage(values);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError("Error parsing GPS message: {message}", ex.Message);
            }
        else if (values.ContainsKey("SA"))
            ParseAvStatusMessage(values);
        else if (values.ContainsKey("RES"))
            ParseResMessage(values);
        else
            logger.LogError("Unknown message type received: {message}", message);

        ParseRawMessage(values, message);
    }

    private void ParseGpsMessage(Dictionary<string, string> values)
    {
        logger.LogDebug("Parsing GPS message: {values}", values);

        GpsDataUpdated?.Invoke(this, new GpsData
        {
            Latitude = ParseDouble(values["LAT"]),
            Longitude = ParseDouble(values["LNG"]),
            AltitudeMetres = ParseDouble(values["ALM"]),
            AltitudeKilometres = ParseDouble(values["AKM"]),
            MetresPerSecond = ParseDouble(values["MPS"]),
            KilometresPerHour = ParseDouble(values["KMH"]),
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
        logger.LogDebug("Parsing AV Status message: {values}", values);

        AvDataUpdated?.Invoke(this, new AvData
        {
            Speed = new ValuePair<double>
            {
                Actual = ParseDouble(values["SA"]), Target = ParseDouble(values["ST"])
            },
            SteeringAngle = new ValuePair<double>
            {
                Actual = ParseSignedInt(values["STA"]), Target = ParseSignedInt(values["STT"])
            },
            BrakeActuation = new ValuePair<double>
            {
                Actual = ParseDouble(values["BRA"]), Target = ParseDouble(values["BRT"])
            },
            MotorMoment = new ValuePair<double>
            {
                Actual = ParseSignedInt(values["MMA"]), Target = ParseSignedInt(values["MMT"])
            },
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
        logger.LogDebug("Parsing RES message: {values}", values);

        ResDataUpdated?.Invoke(this, new ResData
        {
            ResState = ParseBool(values["RES"]),
            K2State = ParseBool(values["K2T"]),
            K3State = ParseBool(values["K3B"]),
            ResRadioQuality = ParseInt(values["RRQ"]),
            ResNodeId = ParseInt("11")
        });
    }

    private DateTime ParseUTCTime(string UTC)
    {
        // If the reported time is equal to this exception, throw an exception as the GPS module hasn't been initialized yet.
        var formatException = "P200000T00:000:00.00";
        if (UTC == formatException) throw new InvalidOperationException("GPS module not initialized yet.");
        var format = "'P'yyyyMMdd'T'HH:mm:ss.ff";
        var parsedDateTime = DateTime.ParseExact(UTC, format, CultureInfo.InvariantCulture);
        // Return the parsed DateTime
        return parsedDateTime;
    }

    private int ParseSignedInt(string value)
    {
        // 128 is 0, below 128 is negative and above 128 is positive
        var translatedValue = int.Parse(value);
        return translatedValue - 128;
    }

    private void ParseRawMessage(Dictionary<string, string> values, string rawMessage)
    {
        logger.LogDebug("Parsing Raw message: {values}", values);
        RawDataUpdated?.Invoke(this, new RawData
        {
            CarId = values["ID"],
            UTCTime = ParseUTCTime(values["UTC"]),
            ConnectionStatus = comPort.IsConnected
        });
    }

    private double ParseDouble(string value)
    {
        return value.StartsWith('#') ? _random.NextDouble() * 100 : double.Parse(value);
    }

    private int ParseInt(string value)
    {
        return value.StartsWith('#') ? _random.Next(0, 5) : int.Parse(value);
    }

    private bool ParseBool(string value)
    {
        return value.StartsWith('#') ? _random.Next() % 2 == 0 : value == "1";
    }

    private void SendHeartbeat()
    {
        try
        {
            logger.LogDebug("Sending heartbeat: {HeartBeatMessage}", HeartBeatMessage);
            comPort.Write(HeartBeatMessage);
        }
        catch (Exception ex)
        {
            logger.LogError("Error sending heartbeat: {message}", ex.Message);
        }
    }
}