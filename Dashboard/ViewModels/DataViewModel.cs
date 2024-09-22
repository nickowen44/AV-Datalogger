using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public class DataViewModel : ViewModelBase
{
    private readonly IDataStore _dataStore;

    public double Speed => _dataStore.AvStatusData?.Speed.Actual ?? 0;
    public double SteeringAngle => _dataStore.AvStatusData?.SteeringAngle.Actual ?? 0;
    public double BrakeActuation => _dataStore.AvStatusData?.BrakeActuation.Actual ?? 0;

    public DataViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;

        _dataStore.AvDataUpdated += OnAvDataChanged;
    }

    public DataViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());
    }

    /// <summary>
    ///     Notifies the view that the AV data has changed.
    /// </summary>
    private void OnAvDataChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("AV Data Updated in MainViewModel");

        OnPropertyChanged(nameof(Speed));
        OnPropertyChanged(nameof(SteeringAngle));
        OnPropertyChanged(nameof(BrakeActuation));
    }

    /// <summary>
    ///     Handles the cleanup when the view model is no longer needed.
    /// </summary>
    public override void Dispose()
    {
        _dataStore.AvDataUpdated -= OnAvDataChanged;
        
        _dataStore.Dispose();
        
        GC.SuppressFinalize(this);
    }
}