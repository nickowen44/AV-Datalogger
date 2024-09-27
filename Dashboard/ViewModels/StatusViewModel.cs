using System;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class StatusViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;

    public StatusViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;
        _dataStore.AvDataUpdated += OnDataChanged;
    }

    public double SpeedActual => _dataStore.AvStatusData?.Speed.Actual ?? 0;

    public double SteeringAngleActual => _dataStore.AvStatusData?.SteeringAngle.Actual ?? 0;

    public double BrakePressureActual => _dataStore.AvStatusData?.BrakeActuation.Actual ?? 0;

    public double MotorMomentActual => _dataStore.AvStatusData?.MotorMoment.Actual ?? 0;

    public double SpeedTarget => _dataStore.AvStatusData?.Speed.Target ?? 0;

    public double SteeringAngleTarget => _dataStore.AvStatusData?.SteeringAngle.Target ?? 0;

    public double BrakePressureTarget => _dataStore.AvStatusData?.BrakeActuation.Target ?? 0;

    public double MotorMomentTarget => _dataStore.AvStatusData?.MotorMoment.Target ?? 0;

    public int MissionIndicator => _dataStore.AvStatusData?.MissionIndicator ?? 0;

    public int EmergencyBrakeState => _dataStore.AvStatusData?.EmergencyBrakeState ?? 0;

    public bool ServiceBrakeState => _dataStore.AvStatusData?.ServiceBrakeState ?? false;

    private void OnDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(SpeedActual));
        OnPropertyChanged(nameof(SteeringAngleActual));
        OnPropertyChanged(nameof(BrakePressureActual));
        OnPropertyChanged(nameof(MotorMomentActual));
        OnPropertyChanged(nameof(SpeedTarget));
        OnPropertyChanged(nameof(SteeringAngleTarget));
        OnPropertyChanged(nameof(BrakePressureTarget));
        OnPropertyChanged(nameof(MotorMomentTarget));
        OnPropertyChanged(nameof(MissionIndicator));
        OnPropertyChanged(nameof(EmergencyBrakeState));
        OnPropertyChanged(nameof(ServiceBrakeState));
    }

    public void Dispose()
    {
        _dataStore.Dispose();
    }
}