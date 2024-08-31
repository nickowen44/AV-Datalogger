using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class ScrutineeringViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;
    
    // public int AutonomousState => _dataStore.AutonomousState;
    public double SteeringAngle => _dataStore.SteeringAngle;
    // public int ServiceBrakeState => _dataStore.ServiceBrakeState;
    // public int EmergencyBrakeState => _dataStore.EmergencyBrakeState;
    // public int AutonomousMissionIndicator => _dataStore.AutonomousMissionIndicator;
    
    public ScrutineeringViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.DataUpdated += OnDataChanged;
    }

    public ScrutineeringViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());
    }

    /// <summary>
    ///     Notifies the view that the data has changed.
    /// </summary>
    private void OnDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SteeringAngle));
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