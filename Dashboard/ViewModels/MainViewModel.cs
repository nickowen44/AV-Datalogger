using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;

    public double SpeedActual => _dataStore.SpeedActual;
    
    public double SpeedTarget => _dataStore.SpeedTarget;

    public double SteeringAngleActual => _dataStore.SteeringAngleActual;
    
    public double SteeringAngleTarget => _dataStore.SteeringAngleTarget;

    public double BrakePressureActual => _dataStore.BrakePressureActual;

    public double BrakePressureTarget => _dataStore.BrakePressureTarget;

    public bool RemoteEmergencyStopStatus => _dataStore.RemoteEmergencyStopStatus;

    public MainViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.DataUpdated += OnDataChanged;
    }

    public MainViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());
    }

    /// <summary>
    ///     Notifies the view that the data has changed.
    /// </summary>
    private void OnDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SpeedActual));
        OnPropertyChanged(nameof(SpeedTarget));
        OnPropertyChanged(nameof(SteeringAngleActual));
        OnPropertyChanged(nameof(SteeringAngleTarget));
        OnPropertyChanged(nameof(BrakePressureActual));
        OnPropertyChanged(nameof(BrakePressureTarget));
        OnPropertyChanged(nameof(RemoteEmergencyStopStatus));
    }

    /// <summary>
    ///     Handles the cleanup when the view model is no longer needed.
    /// </summary>
    public void Dispose()
    {
        _dataStore.DataUpdated -= OnDataChanged;

        _dataStore.Dispose();

        GC.SuppressFinalize(this);
    }
}