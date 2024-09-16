using System;
using Dashboard.Connectors;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
    public event EventHandler? GpsDataUpdated;
    public event EventHandler? AvDataUpdated;
    public event EventHandler? ResDataUpdated;

    public GpsData? GpsData { get; private set; }
    public AvData? AvStatusData { get; private set; }
    public ResData? ResData { get; private set; }

    private readonly IConnector _connector;

    public DataStore(IConnector connector)
    {
        _connector = connector;

        _connector.GpsDataUpdated += OnGpsDataUpdated;
        _connector.AvDataUpdated += OnAvDataUpdated;
        _connector.ResDataUpdated += OnResDataUpdated;

        _connector.Start();
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

    public void Dispose()
    {
        // Stop the connector
        _connector.GpsDataUpdated -= OnGpsDataUpdated;
        _connector.AvDataUpdated -= OnAvDataUpdated;
        _connector.ResDataUpdated -= OnResDataUpdated;

        _connector.Stop();

        GC.SuppressFinalize(this);
    }
}