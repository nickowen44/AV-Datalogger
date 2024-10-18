using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Dashboard.Connectors;
using Dashboard.Connectors.Serial;
using Dashboard.Serialisation;
using Dashboard.Utils;
using Microsoft.Extensions.Logging;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
    private readonly IConnectorFactory _connectorFactory;
    private readonly IDataSerialisationFactory _dataSerialiserFactory;

    // Keep a buffer until the UI is ready to display the message
    private readonly List<string> _logBuffer = [];
    private readonly ILogger<DataStore> _logger;

    private IConnector? _connector;
    private IDataSerialiser? _dataSerialiser;
    private Timer? _serialisationTimer;

    public DataStore(IConnectorFactory connectorFactory, ILogger<DataStore> logger, IDataSerialisationFactory factory)
    {
        _connectorFactory = connectorFactory;
        _logger = logger;
        _dataSerialiserFactory = factory;

        LoggingConfig.LogEventSink.LogMessageReceived += OnLogMessageReceived;

        _logger.LogDebug("DataStore created");
    }

    public event EventHandler? GpsDataUpdated;
    public event EventHandler? AvDataUpdated;
    public event EventHandler? ResDataUpdated;
    public event EventHandler? RawDataUpdated;
    public event EventHandler<bool>? HeartBeatUpdated;
    public event EventHandler<string>? ConsoleMessageUpdated;

    public bool? HeartBeat { get; private set; }
    public GpsData? GpsData { get; private set; }
    public AvData? AvStatusData { get; private set; }
    public ResData? ResData { get; private set; }
    public RawData? RawData { get; private set; }

    public bool Connect(IConnectorArgs args)
    {
        try
        {
            SetupConnector(args);

            if (args is SerialConnectorArgs { SaveToCsv: true })
                try
                {
                    _dataSerialiser = _dataSerialiserFactory.CreateDataSerialiser();

                    _serialisationTimer = CreateSerialisationTimer();
                    _serialisationTimer.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create data serialiser");
                }

            // Now start the connection
            _connector?.Start(args);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start connection");

            _connector?.Stop();
            return false;
        }
    }

    public void Disconnect()
    {
        _connector?.Stop();

        _serialisationTimer?.Stop();
        _serialisationTimer?.Dispose();

        _dataSerialiser?.Dispose();
        _dataSerialiser = null;
    }

    public void FlushLogBuffer()
    {
        foreach (var message in _logBuffer) ConsoleMessageUpdated?.Invoke(this, message);

        _logBuffer.Clear();
    }

    public void Dispose()
    {
        _logger.LogDebug("DataStore disposed");

        // Stop the connector
        if (_connector is not null)
        {
            _connector.GpsDataUpdated -= OnGpsDataUpdated;
            _connector.AvDataUpdated -= OnAvDataUpdated;
            _connector.ResDataUpdated -= OnResDataUpdated;
            _connector.RawDataUpdated -= OnRawDataUpdated;

            _connector.Stop();
        }

        // Stop the serialisation timer
        _serialisationTimer?.Stop();
        _serialisationTimer?.Dispose();

        // Dispose of the data serialiser
        _dataSerialiser?.Dispose();
        _dataSerialiser = null;

        GC.SuppressFinalize(this);
    }

    private void SetupConnector(IConnectorArgs args)
    {
        _connector = _connectorFactory.CreateConnector(args);

        _connector.GpsDataUpdated += OnGpsDataUpdated;
        _connector.AvDataUpdated += OnAvDataUpdated;
        _connector.ResDataUpdated += OnResDataUpdated;
        _connector.RawDataUpdated += OnRawDataUpdated;
        _connector.HeartBeatUpdated += OnHeartbeatUpdated;
    }

    private void OnLogMessageReceived(object? sender, string e)
    {
        // Ensure we have at least one subscriber, otherwise fill the buffer
        if (ConsoleMessageUpdated == null)
        {
            _logBuffer.Add(e);
            return;
        }

        ConsoleMessageUpdated?.Invoke(this, e);
    }

    private void OnHeartbeatUpdated(object? sender, bool isReceived)
    {
        _logger.LogDebug("Heartbeat received: {isReceived}", isReceived);
        HeartBeat = isReceived;
        HeartBeatUpdated?.Invoke(this, isReceived);
    }

    private void OnGpsDataUpdated(object? sender, GpsData e)
    {
        _logger.LogDebug("GPS data received: {e}", e);
        GpsData = e;

        GpsDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnAvDataUpdated(object? sender, AvData e)
    {
        _logger.LogDebug("AV data received: {e}", e);
        AvStatusData = e;

        AvDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnResDataUpdated(object? sender, ResData e)
    {
        _logger.LogDebug("RES data received: {e}", e);
        ResData = e;

        ResDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnRawDataUpdated(object? sender, RawData e)
    {
        _logger.LogDebug("Raw data received: {e}", e);
        RawData = e;

        RawDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private Timer CreateSerialisationTimer()
    {
        // Run the serialisation every second
        var timer = new Timer(1000);
        timer.Elapsed += async (_, __) => await DoSerialisation();

        return timer;
    }

    private async Task DoSerialisation()
    {
        if (_dataSerialiser is null) return;
        if (GpsData is null || AvStatusData is null || ResData is null || RawData is null) return;

        await _dataSerialiser.Write(GpsData, AvStatusData, ResData, RawData);
    }
}