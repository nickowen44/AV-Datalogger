namespace Dashboard.Serialisation;

public interface IDataSerialisationFactory
{
    /// <summary>
    ///     Allowing for runtime creation (or dependency injection) of a data serialiser.
    ///     Primarily used for testing purposes.
    /// </summary>
    /// <returns>An <see cref="IDataSerialiser" /></returns>
    public IDataSerialiser CreateDataSerialiser();
}