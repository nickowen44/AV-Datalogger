﻿using System;
using Dashboard.Connectors;

namespace Dashboard.Models;

public class DataStore : IDataStore, IDisposable
{
    private readonly IConnector _connector;

    public DataStore(IConnector connector)
    {
        _connector = connector;
        _connector.DataUpdated += OnDataUpdated;

        _connector.Start();
    }

    public event EventHandler? DataUpdated;

    public double Speed { get; private set; }
    public double SteeringAngle { get; private set; }
    public double BrakePressure { get; private set; }

    public void UpdateData(double speed, double steeringAngle, double brakePressure)
    {
        Speed = speed;
        SteeringAngle = steeringAngle;
        BrakePressure = brakePressure;

        DataUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _connector.DataUpdated -= OnDataUpdated;

        _connector.Stop();

        GC.SuppressFinalize(this);
    }

    private void OnDataUpdated(object? sender, DataUpdatedEventArgs e)
    {
        UpdateData(e.Speed, e.SteeringAngle, e.BrakePressure);
    }
}