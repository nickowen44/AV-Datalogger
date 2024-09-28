using System;
using Dashboard.Connectors;
using Dashboard.Models;

namespace Dashboard.ViewModels;

public partial class StatusViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;
    public event Action<bool>? RESDataUpdated;
    public StatusViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;
        _dataStore.AvDataUpdated += OnAVDataChanged;
        _dataStore.ResDataUpdated += OnResDataChanged;
    }

    public StatusViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new DummyConnector());
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
    public bool RemoteEmergency => _dataStore.ResData?.ResState ?? true;

    private void OnAVDataChanged(object? sender, EventArgs e)
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
    private void OnResDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(RemoteEmergency));
        RESDataUpdated?.Invoke(RemoteEmergency);
    }

    public void Dispose()
    {
        _dataStore.Dispose();
    }
}