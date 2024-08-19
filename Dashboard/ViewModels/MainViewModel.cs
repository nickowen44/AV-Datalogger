using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IDataStore _dataStore;
    private readonly IConnector _connector;

    public double Speed => _dataStore.Speed;
    public double SteeringAngle => _dataStore.SteeringAngle;
    public double BrakePressure => _dataStore.BrakePressure;

    public MainViewModel(IDataStore dataStore, IConnector connector)
    {
        _dataStore = dataStore;
        _connector = connector;

        _dataStore.DataUpdated += OnDataChanged;
        _connector.DataUpdated += OnDataUpdated;

        _connector.Start();
    }

    public MainViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore();
        _connector = new DummyConnector();
    }

    /// <summary>
    ///     Notifies the data store that new data has arrived.
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event containing the new data</param>
    private void OnDataUpdated(object? sender, DataUpdatedEventArgs e)
    {
        _dataStore.UpdateData(e.Speed, e.SteeringAngle, e.BrakePressure);
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
        _connector.DataUpdated -= OnDataUpdated;

        _connector.Stop();
    }
}