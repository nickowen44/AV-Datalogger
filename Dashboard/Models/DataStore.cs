using System;
using Dashboard.Connectors;

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

    public DataStore(IConnector connector)
    {
        _connector = connector;

        _connector.GpsDataUpdated += OnGpsDataUpdated;
        _connector.AvDataUpdated += OnAvDataUpdated;
        _connector.ResDataUpdated += OnResDataUpdated;
        _connector.RawDataUpdated += OnRawDataUpdated;
        _connector.HeartBeatUpdated += OnHeartbeatUpdated;
        _connector.Start();
    }
    private void OnHeartbeatUpdated(object? sender, bool isReceived)
    {
        HeartBeat = isReceived;
        HeartBeatUpdated?.Invoke(this, isReceived);
    }

    private void OnGpsDataUpdated(object? sender, GpsData e)
    {
        GpsData = e;

        GpsDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnAvDataUpdated(object? sender, AvData e)
    {
        AvStatusData = e;

        AvDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnResDataUpdated(object? sender, ResData e)
    {
        ResData = e;

        ResDataUpdated?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnRawDataUpdated(object? sender, RawData e)
    {
        RawData = e;

        RawDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        // Stop the connector
        _connector.GpsDataUpdated -= OnGpsDataUpdated;
        _connector.AvDataUpdated -= OnAvDataUpdated;
        _connector.ResDataUpdated -= OnResDataUpdated;
        _connector.RawDataUpdated -= OnRawDataUpdated;

        _connector.Stop();

        GC.SuppressFinalize(this);
    }
}