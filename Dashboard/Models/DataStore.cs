using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Dashboard.Connectors;
using Dashboard.Serialisation;
using Dashboard.Utils;
using Microsoft.Extensions.Logging;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
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

    private readonly IConnector _connector;
    private readonly ILogger<DataStore> _logger;
    private readonly IDataSerialisationFactory _dataSerialiserFactory;
    private IDataSerialiser? _dataSerialiser;
    private Timer? _serialisationTimer;

    public DataStore(IConnector connector, ILogger<DataStore> logger, IDataSerialisationFactory factory)
    {
        _connector = connector;
        _logger = logger;
        _dataSerialiserFactory = factory;

        LoggingConfig.LogEventSink.LogMessageReceived += OnLogMessageReceived;

        _logger.LogDebug("DataStore created");

        _connector.GpsDataUpdated += OnGpsDataUpdated;
        _connector.AvDataUpdated += OnAvDataUpdated;
        _connector.ResDataUpdated += OnResDataUpdated;
        _connector.RawDataUpdated += OnRawDataUpdated;
        _connector.HeartBeatUpdated += OnHeartbeatUpdated;
    }

    public bool startConnection(string portName, bool saveToCsv)
    {
        try
        {
            _connector.Start(portName);

            if (saveToCsv)
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

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start connection");

            _connector.Stop();
            return false;
        }
    }

    public void disconnect()
    {
        _connector.Stop();

        _serialisationTimer?.Stop();
        _serialisationTimer?.Dispose();

        _dataSerialiser?.Dispose();
        _dataSerialiser = null;
    }

    // Keep a buffer until the UI is ready to display the message
    private readonly List<string> _logBuffer = [];

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

    public void FlushLogBuffer()
    {
        foreach (var message in _logBuffer) ConsoleMessageUpdated?.Invoke(this, message);

        _logBuffer.Clear();
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

    public void Dispose()
    {
        _logger.LogDebug("DataStore disposed");

        // Stop the connector
        _connector.GpsDataUpdated -= OnGpsDataUpdated;
        _connector.AvDataUpdated -= OnAvDataUpdated;
        _connector.ResDataUpdated -= OnResDataUpdated;
        _connector.RawDataUpdated -= OnRawDataUpdated;

        _connector.Stop();

        // Stop the serialisation timer
        _serialisationTimer?.Stop();
        _serialisationTimer?.Dispose();

        // Dispose of the data serialiser
        _dataSerialiser?.Dispose();
        _dataSerialiser = null;

        GC.SuppressFinalize(this);
    }
}