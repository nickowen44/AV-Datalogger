using Dashboard.Serialisation.Csv;

namespace Dashboard.Serialisation;

public class DataSerialisationFactory : IDataSerialisationFactory
{
    public IDataSerialiser CreateDataSerialiser()
    {
        return new CsvDataSerialiser();
    }
}