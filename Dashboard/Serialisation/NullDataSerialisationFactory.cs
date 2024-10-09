using System.Threading.Tasks;
using Dashboard.Models;

namespace Dashboard.Serialisation;

internal class NullWrapper : IDataSerialiser
{
    public static readonly NullWrapper Instance = new();

    private NullWrapper()
    {
    }

    /// <summary>
    ///     Method does nothing, as the NullWrapper is a dummy class.
    /// </summary>
    public Task Write(GpsData gpsData, AvData avData, ResData resData, RawData rawData)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Method does nothing, as the NullWrapper is a dummy class.
    /// </summary>
    public void Dispose()
    {
    }
}

public class NullDataSerialisationFactory : IDataSerialisationFactory
{
    public static readonly IDataSerialisationFactory Instance = new NullDataSerialisationFactory();

    private NullDataSerialisationFactory()
    {
    }

    public IDataSerialiser CreateDataSerialiser()
    {
        return NullWrapper.Instance;
    }
}