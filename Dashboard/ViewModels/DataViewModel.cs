﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class DataViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;
    
    public double Speed => _dataStore.Speed;
    public double SteeringAngle => _dataStore.SteeringAngle;
    public double BrakePressure => _dataStore.BrakePressure;

    public DataViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.DataUpdated += OnDataChanged;
        
    }

    public DataViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());
    }
    
    /// <summary>
    ///     Notifies the view that the data has changed.
    /// </summary>
    private void OnDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Speed));
        OnPropertyChanged(nameof(SteeringAngle));
        OnPropertyChanged(nameof(BrakePressure));
    }
    
    /// <summary>
    ///     Handles the cleanup when the view model is no longer needed.
    /// </summary>
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
                _dataStore.DataUpdated -= OnDataChanged;
                _dataStore.Dispose();
                Console.WriteLine("Called D2");
            }
            _disposed = true;
        }
    }

    ~DataViewModel()
    {
        Dispose(false);
    }
}

