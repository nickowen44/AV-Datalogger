using System;
using Dashboard.Connectors;
using Dashboard.Models;
using Dashboard.Serialisation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dashboard.ViewModels;

public class StatusViewModel : ViewModelBase, IDisposable
{
    private readonly IDataStore _dataStore;

    [ActivatorUtilitiesConstructor]
    public StatusViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;
        _dataStore.AvDataUpdated += OnAvDataChanged;
        _dataStore.ResDataUpdated += OnResDataChanged;
        _dataStore.GpsDataUpdated += OnGpsDataChanged;
    }

    public StatusViewModel()
    {
        // This constructor is used for design-time data, so we don't need to start the connector
        _dataStore = new DataStore(new NullConnectorFactory(), NullLogger<DataStore>.Instance,
            NullDataSerialisationFactory.Instance);
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

    public bool RemoteEmergency => _dataStore.ResData?.ResState ?? false;

    public int SatCount => _dataStore.GpsData?.SatCount ?? 0;

    public double Hdop => _dataStore.GpsData?.Hdop ?? 0;

    public int HdopFixAge => _dataStore.GpsData?.HdopFixAge ?? 0;

    public int HVal => _dataStore.GpsData?.HVal ?? 0;

    public int SatFixAge => _dataStore.GpsData?.SatFixAge ?? 0;

    public int SpeedFixAge => _dataStore.GpsData?.SpeedFixAge ?? 0;

    public int AltFixAge => _dataStore.GpsData?.AltFixAge ?? 0;

    public double AltitudeMetres => _dataStore.GpsData?.AltitudeMetres ?? 0;

    public double AltitudeKilometres => _dataStore.GpsData?.AltitudeKilometres ?? 0;

    public double MetresPerSecond => _dataStore.GpsData?.MetresPerSecond ?? 0;

    public double KilometresPerHour => _dataStore.GpsData?.KilometresPerHour ?? 0;

    public double Latitude => _dataStore.GpsData?.Latitude ?? 0;

    public double Longitude => _dataStore.GpsData?.Longitude ?? 0;

    public bool ResState => _dataStore.ResData?.ResState ?? false;

    public bool K2State => _dataStore.ResData?.K2State ?? false;

    public bool K3State => _dataStore.ResData?.K3State ?? false;

    public int ResRadioQuality => _dataStore.ResData?.ResRadioQuality ?? 0;

    public int ResNodeId => _dataStore.ResData?.ResNodeId ?? 0;

    public override void Dispose()
    {
        _dataStore.AvDataUpdated -= OnAvDataChanged;
        _dataStore.ResDataUpdated -= OnResDataChanged;
        _dataStore.GpsDataUpdated -= OnGpsDataChanged;

        GC.SuppressFinalize(this);
    }

    public event Action<bool>? RESDataUpdated;

    private void OnAvDataChanged(object? sender, EventArgs e)
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

    private void OnGpsDataChanged(object? sender, EventArgs e)
    {
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

    private void OnResDataChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(RemoteEmergency));
        OnPropertyChanged(nameof(ResState));
        OnPropertyChanged(nameof(K2State));
        OnPropertyChanged(nameof(K3State));
        OnPropertyChanged(nameof(ResRadioQuality));
        OnPropertyChanged(nameof(ResNodeId));
        RESDataUpdated?.Invoke(RemoteEmergency);
    }
}