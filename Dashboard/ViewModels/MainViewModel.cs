using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;

    public double Speed => _dataStore.Speed;
    public double SteeringAngle => _dataStore.SteeringAngle;
    public double BrakePressure => _dataStore.BrakePressure;

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
        OnPropertyChanged(nameof(Speed));
        OnPropertyChanged(nameof(SteeringAngle));
        OnPropertyChanged(nameof(BrakePressure));
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