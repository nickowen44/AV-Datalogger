using System;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class StatusViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;

    public StatusViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;
        _dataStore.OnDataUpdated(_dataStore, EventArgs.Empty);
    }

    public string AutonomousMissionIndicator => _dataStore.AutonomousMissionIndicator;

    public double SpeedActual => _dataStore.SpeedActual;

    public double SpeedTarget => _dataStore.SpeedTarget;

    public double SteeringAngleActual => _dataStore.SteeringAngleActual;

    public double SteeringAngleTarget => _dataStore.SteeringAngleTarget;

    public double BrakePressureActual => _dataStore.BrakePressureActual;

    public double BrakePressureTarget => _dataStore.BrakePressureTarget;

    public bool RemoteEmergencyStopStatus => _dataStore.RemoteEmergencyStopStatus;

    public void Dispose()
    {
        _dataStore.Dispose();
    }
}