using System;
using Dashboard.Connectors;
using Microsoft.Extensions.Logging;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
    public event EventHandler? GpsDataUpdated;
    public event EventHandler? AvDataUpdated;
    public event EventHandler? ResDataUpdated;
    public event EventHandler? RawDataUpdated;
    public event EventHandler<bool>? HeartBeatUpdated;

    public bool? HeartBeat { get; private set; }
    public GpsData? GpsData { get; private set; }
    public AvData? AvStatusData { get; private set; }
    public ResData? ResData { get; private set; }
    public RawData? RawData { get; private set; }

    private readonly IConnector _connector;
    private readonly ILogger<DataStore> _logger;

    public DataStore(IConnector connector, ILogger<DataStore> logger)
    {
        _connector = connector;
        _logger = logger;

        _logger.LogDebug("DataStore created");

        _connector.GpsDataUpdated += OnGpsDataUpdated;
        _connector.AvDataUpdated += OnAvDataUpdated;
        _connector.ResDataUpdated += OnResDataUpdated;
        _connector.RawDataUpdated += OnRawDataUpdated;
        _connector.HeartBeatUpdated += OnHeartbeatUpdated;

        _connector.Start();
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

    public void Dispose()
    {
        _logger.LogDebug("DataStore disposed");
        
        // Stop the connector
        _connector.GpsDataUpdated -= OnGpsDataUpdated;
        _connector.AvDataUpdated -= OnAvDataUpdated;
        _connector.ResDataUpdated -= OnResDataUpdated;
        _connector.RawDataUpdated -= OnRawDataUpdated;

        _connector.Stop();

        GC.SuppressFinalize(this);
    }
}