using System;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class StatusViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;

    public StatusViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;
        _dataStore.PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName);
    }

    public string AutonomousMissionIndicator => _dataStore.AutonomousMissionIndicator;

    public int SpeedActual => _dataStore.SpeedActual;

    public int SpeedTarget => _dataStore.SpeedTarget;

    public int SteeringAngleActual => _dataStore.SteeringAngleActual;

    public int SteeringAngleTarget => _dataStore.SteeringAngleTarget;

    public int BrakePressureActual => _dataStore.BrakePressureActual;

    public int BrakePressureTarget => _dataStore.BrakePressureTarget;

    public bool RemoteEmergencyStopStatus => _dataStore.RemoteEmergencyStopStatus;

    public void Dispose()
    {
        _dataStore.PropertyChanged -= (sender, args) => OnPropertyChanged(args.PropertyName);
    }
}