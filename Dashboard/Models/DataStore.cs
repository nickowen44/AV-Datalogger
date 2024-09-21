using System;
using Dashboard.Connectors;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
    public event EventHandler? GpsDataUpdated;
    public event EventHandler? AvDataUpdated;
    public event EventHandler? ResDataUpdated;
    public void OnDataUpdated(object? sender, EventArgs e)
    {
        // Implementation of the interface method
        if (e is DataUpdatedEventArgs dataUpdatedEventArgs)
        {
            UpdateData(dataUpdatedEventArgs.AutonomousMissionIndicator, dataUpdatedEventArgs.SpeedActual, dataUpdatedEventArgs.SpeedTarget, dataUpdatedEventArgs.SteeringAngleActual, dataUpdatedEventArgs.SteeringAngleTarget, dataUpdatedEventArgs.BrakePressureActual, dataUpdatedEventArgs.BrakePressureTarget, dataUpdatedEventArgs.RemoteEmergencyStopStatus);
        }
    }

    public event EventHandler? DataUpdated;

    public GpsData? GpsData { get; private set; }
    public AvData? AvStatusData { get; private set; }
    public ResData? ResData { get; private set; }
    public string AutonomousMissionIndicator { get; private set; }
    public double SpeedActual { get; private set; }
    public double SpeedTarget { get; private set; }
    public double SteeringAngleActual { get; private set; }
    public double SteeringAngleTarget { get; private set; }
    public double BrakePressureActual { get; private set; }
    public double BrakePressureTarget { get; private set; }
    public bool RemoteEmergencyStopStatus { get; private set; }

    private readonly IConnector _connector;

    public DataStore(IConnector connector)
    {
        _connector = connector;

        _connector.GpsDataUpdated += OnGpsDataUpdated;
        _connector.AvDataUpdated += OnAvDataUpdated;
        _connector.ResDataUpdated += OnResDataUpdated;
        _connector.DataUpdated += OnDataUpdated;

        AutonomousMissionIndicator = string.Empty; // Initialize with a default value

        _connector.Start();
    }

    private void OnGpsDataUpdated(object? sender, GpsData e)
    public void UpdateData(string autonomousMissionIndicator, double speedActual, double speedTarget, double steeringAngleActual, double steeringAngleTarget, double brakePressureActual, double brakePressureTarget, bool remoteEmergencyStopStatus)
    {
        GpsData = e;

        GpsDataUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void OnAvDataUpdated(object? sender, AvData e)
    {
        AvStatusData = e;

        AvDataUpdated?.Invoke(this, EventArgs.Empty);
        AutonomousMissionIndicator = autonomousMissionIndicator;
        SpeedActual = speedActual;
        SpeedTarget = speedTarget;
        SteeringAngleActual = steeringAngleActual;
        SteeringAngleTarget = steeringAngleTarget;
        BrakePressureActual = brakePressureActual;
        BrakePressureTarget = brakePressureTarget;
        RemoteEmergencyStopStatus = remoteEmergencyStopStatus;

        DataUpdated?.Invoke(this, EventArgs.Empty);
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