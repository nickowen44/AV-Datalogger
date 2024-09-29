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

    public int SatCount => _dataStore.GpsData?.SatCount ?? 0;
    
    public double Hdop => _dataStore.GpsData?.Hdop ?? 0;
    
    public int HdopFixAge => _dataStore.GpsData?.HdopFixAge ?? 0;
    
    public int HVal  => _dataStore.GpsData?.HVal ?? 0;
    
    public int SatFixAge  => _dataStore.GpsData?.SatFixAge ?? 0;
    
    public int SpeedFixAge  => _dataStore.GpsData?.SpeedFixAge ?? 0;
    
    public int AltFixAge  => _dataStore.GpsData?.AltFixAge ?? 0;
    
    public double AltitudeMetres  => _dataStore.GpsData?.AltitudeMetres ?? 0;
    
    public double AltitudeKilometres => _dataStore.GpsData?.AltitudeKilometres ?? 0;
    
    public double MetresPerSecond => _dataStore.GpsData?.MetresPerSecond ?? 0;
    
    public double KilometresPerHour => _dataStore.GpsData?.KilometresPerHour ?? 0;
    
    public double Latitude => _dataStore.GpsData?.Latitude ?? 0;
    
    public double Longitude => _dataStore.GpsData?.Longitude ?? 0;

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
        OnPropertyChanged(nameof(SatCount));
        OnPropertyChanged(nameof(Hdop));
        OnPropertyChanged(nameof(HdopFixAge));
        OnPropertyChanged(nameof(HVal));
        OnPropertyChanged(nameof(SatFixAge));
        OnPropertyChanged(nameof(SpeedFixAge));
        OnPropertyChanged(nameof(AltitudeMetres));
        OnPropertyChanged(nameof(AltitudeKilometres));
        OnPropertyChanged(nameof(MetresPerSecond));
        OnPropertyChanged(nameof(KilometresPerHour));
        OnPropertyChanged(nameof(Latitude));
        OnPropertyChanged(nameof(Longitude));
    }

    public void Dispose()
    {
        _dataStore.Dispose();
    }
}