using System;
using Dashboard.Connectors;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
    public event EventHandler? DataUpdated;

    public double Speed { get; private set; }
    public double SteeringAngle { get; private set; }
    public double BrakePressure { get; private set; }

    private readonly IConnector _connector;

    public DataStore(IConnector connector)
    {
        _connector = connector;
        _connector.DataUpdated += OnDataUpdated;

        _connector.Start();
    }

    public void UpdateData(double speed, double steeringAngle, double brakePressure)
    {
        Speed = speed;
        SteeringAngle = steeringAngle;
        BrakePressure = brakePressure;

        DataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnDataUpdated(object? sender, DataUpdatedEventArgs e)
    {
        UpdateData(e.Speed, e.SteeringAngle, e.BrakePressure);
    }
    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _connector.DataUpdated -= OnDataUpdated;

                // Stop the connector
                _connector.Stop();
                    
                // Dispose of the connector if it's disposable
                if (_connector is IDisposable disposableConnector)
                {
                    disposableConnector.Dispose();
                }
            }
            _disposed = true;
        }
    }

    ~DataStore()
    {
        Dispose(false);
    }
}
