using System;
using System.IO;
using System.Threading;
using Dashboard.Models;
using Dashboard.Serialisation.Csv;
using Dashboard.Utils;
using Microsoft.Extensions.Logging;

namespace Dashboard.Connectors.Csv;

public class CsvConnector(ILogger<CsvConnector> logger) : IConnector
{
    private StreamReader? _reader;

    private Thread? _readThread;
    private bool _shouldStop;
    public event EventHandler<GpsData>? GpsDataUpdated;
    public event EventHandler<AvData>? AvDataUpdated;
    public event EventHandler<ResData>? ResDataUpdated;
    public event EventHandler<RawData>? RawDataUpdated;
    public event EventHandler<bool>? HeartBeatUpdated;

    public void Start(IConnectorArgs args)
    {
        if (args is not CsvConnectorArgs csvArgs)
        {
            logger.LogError("Invalid arguments passed to Csv Connector");
            return;
        }

        // Open the file
        _reader = OpenReader(csvArgs.FilePath);
        if (_reader == null)
        {
            logger.LogError("Failed to open log replay file");
            return;
        }

        // Validate the file header
        var header = _reader.ReadLine();
        if (header != CsvDataSerialiser.Header)
        {
            logger.LogError("Invalid file header");
            return;
        }

        _readThread = new Thread(() =>
        {
            logger.LogInformation("Starting CSV read thread");

            while (!_shouldStop)
            {
                var line = _reader.ReadLine();

                if (line == null)
                {
                    // We are at EOF, loop back to the start of the file
                    _reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    // and skip the header
                    _reader.ReadLine();

                    continue;
                }

                ParseData(line);

                // Sleep for a bit
                Thread.Sleep(1000);
            }

            logger.LogInformation("Stopping CSV read thread");

            // Dispose of the reader when we're done
            _reader?.Close();
            _reader?.Dispose();
        });

        _readThread.Start();
    }

    public void Stop()
    {
        _shouldStop = true;
    }

    private StreamReader? OpenReader(string filePath)
    {
        try
        {
            return new StreamReader(filePath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to open log replay file");
        }

        return null;
    }

    private void ParseData(string line)
    {
        var parts = line.Split(',');
        if (parts.Length != 40)
        {
            logger.LogError("Invalid data format");
            return;
        }

        var rawData = new RawData
        {
            CarId = parts[0],
            UTCTime = Time.ParseUtcTime(parts[1]),
            ConnectionStatus = bool.Parse(parts[2])
        };

        var gpsData = new GpsData
        {
            Latitude = double.Parse(parts[3]),
            Longitude = double.Parse(parts[4]),
            AltitudeMetres = double.Parse(parts[5]),
            AltitudeKilometres = double.Parse(parts[6]),
            MetresPerSecond = double.Parse(parts[7]),
            KilometresPerHour = double.Parse(parts[8]),
            HdopFixAge = int.Parse(parts[9]),
            Hdop = double.Parse(parts[10]),
            HVal = int.Parse(parts[11]),
            SatFixAge = int.Parse(parts[12]),
            SatCount = int.Parse(parts[13]),
            SpeedFixAge = int.Parse(parts[14]),
            AltFixAge = int.Parse(parts[15])
        };

        var avData = new AvData
        {
            Speed = new ValuePair<double>
            {
                Actual = double.Parse(parts[16]),
                Target = double.Parse(parts[17])
            },
            SteeringAngle = new ValuePair<double>
            {
                Actual = double.Parse(parts[18]),
                Target = double.Parse(parts[19])
            },
            BrakeActuation = new ValuePair<double>
            {
                Actual = double.Parse(parts[20]),
                Target = double.Parse(parts[21])
            },
            MotorMoment = new ValuePair<double>
            {
                Actual = double.Parse(parts[22]),
                Target = double.Parse(parts[23])
            },
            LateralAcceleration = double.Parse(parts[24]),
            LongitudinalAcceleration = double.Parse(parts[25]),
            YawRate = double.Parse(parts[26]),
            AutonomousSystemState = int.Parse(parts[27]),
            EmergencyBrakeState = int.Parse(parts[28]),
            MissionIndicator = int.Parse(parts[29]),
            SteeringState = bool.Parse(parts[30]),
            ServiceBrakeState = bool.Parse(parts[31]),
            LapCount = int.Parse(parts[32]),
            ConeCountPerLap = int.Parse(parts[33]),
            ConeCountTotal = int.Parse(parts[34])
        };

        var resData = new ResData
        {
            ResState = bool.Parse(parts[35]),
            K2State = bool.Parse(parts[36]),
            K3State = bool.Parse(parts[37]),
            ResRadioQuality = int.Parse(parts[38]),
            ResNodeId = int.Parse(parts[39])
        };

        // Raise the appropriate events
        RawDataUpdated?.Invoke(this, rawData);
        GpsDataUpdated?.Invoke(this, gpsData);
        AvDataUpdated?.Invoke(this, avData);
        ResDataUpdated?.Invoke(this, resData);
    }
}