using System.Threading.Tasks;
using Dashboard.Models;

namespace Dashboard.Serialisation;

public interface IDataSerialiser
{
    /// <summary>
    ///     Writes the data to the CSV file, including GPS, AV, and Res data.
    /// </summary>
    /// <param name="gpsData">The GPS data to write.</param>
    /// <param name="avData">The AV data to write.</param>
    /// <param name="resData">The Res data to write.</param>
    /// <param name="rawData">The Raw data to write.</param>
    Task Write(GpsData gpsData, AvData avData, ResData resData, RawData rawData);

    /// <summary>
    ///     Disposes of the data serialiser, closing any open streams.
    /// </summary>
    void Dispose();
}