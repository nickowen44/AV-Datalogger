using System;
using System.IO;
using System.Threading.Tasks;
using Dashboard.Models;
using Dashboard.Utils;

namespace Dashboard.Serialisation.Csv;

public class CsvDataSerialiser : IDataSerialiser, IDisposable
{
    public const string Header = "CarId,UTCTime,Connected," +
                                 // GPS
                                 "Latitude,Longitude,AltitudeMetres,AltitudeKilometres,MetresPerSecond,KilometresPerHour,HdopFixAge,Hdop,HVal,SatFixAge,SatCount,SpeedFixAge,AltFixAge," +
                                 // AV Data
                                 "SpeedActual,SpeedTarget,SteeringAngleActual,SteeringAngleTarget,BrakeActuationActual,BrakeActuationTarget,MotorMomentActual,MotorMomentTarget,LateralAcceleration,LongitudinalAcceleration,YawRate,AutonomousSystemState,EmergencyBrakeState,MissionIndicator,SteeringState,ServiceBrakeState,LapCount,ConeCountPerLap,ConeCountTotal," +
                                 // Res Data
                                 "ResState,K2State,K3State,ResRadioQuality,ResNodeId";

    private bool _initialised;

    private StreamWriter? _streamWriter;

    public async Task Write(GpsData gpsData, AvData avData, ResData resData, RawData rawData)
    {
        // If the stream writer hasn't been initialised, do so
        if (!_initialised)
        {
            Initialise(rawData);
            _initialised = true;
        }

        // Format the data
        var data = FormatData(gpsData, avData, resData, rawData);

        // Write the data to the file (stream write can't be null here as it's initialised in Initialise)
        await _streamWriter!.WriteLineAsync(data);
    }

    public void Dispose()
    {
        // Dispose of the stream writer
        _streamWriter?.Dispose();

        GC.SuppressFinalize(this);
    }

    private void Initialise(RawData rawData)
    {
        var currentDate = DateTime.Now;
        // Format the file name based on the car ID and the current time
        var path = Path.Combine(Directory.GetCurrentDirectory(), "runs",
            $"{rawData.CarId}_{currentDate:yyyy-MM-dd}T{currentDate:hhmmss}.csv");

        // Create the directory if it doesn't exist
        var directory = Path.GetDirectoryName(path);
        if (directory != null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // Create the stream writer
        _streamWriter = new StreamWriter(path);

        // Write the header to the file
        _streamWriter.WriteLine(Header);
    }

    private static string FormatData(GpsData gpsData, AvData avData, ResData resData, RawData rawData)
    {
        return $"{rawData.CarId},{Time.FormatUtcTime(rawData.UTCTime)},{rawData.ConnectionStatus}," +
               $"{gpsData.Latitude},{gpsData.Longitude},{gpsData.AltitudeMetres},{gpsData.AltitudeKilometres},{gpsData.MetresPerSecond},{gpsData.KilometresPerHour},{gpsData.HdopFixAge},{gpsData.Hdop},{gpsData.HVal},{gpsData.SatFixAge},{gpsData.SatCount},{gpsData.SpeedFixAge},{gpsData.AltFixAge}," +
               $"{avData.Speed.Actual},{avData.Speed.Target},{avData.SteeringAngle.Actual},{avData.SteeringAngle.Target},{avData.BrakeActuation.Actual},{avData.BrakeActuation.Target},{avData.MotorMoment.Actual},{avData.MotorMoment.Target},{avData.LateralAcceleration},{avData.LongitudinalAcceleration},{avData.YawRate},{avData.AutonomousSystemState},{avData.EmergencyBrakeState},{avData.MissionIndicator},{avData.SteeringState},{avData.ServiceBrakeState},{avData.LapCount},{avData.ConeCountPerLap},{avData.ConeCountTotal}," +
               $"{resData.ResState},{resData.K2State},{resData.K3State},{resData.ResRadioQuality},{resData.ResNodeId}";
    }
}