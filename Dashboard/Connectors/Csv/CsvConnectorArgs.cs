namespace Dashboard.Connectors.Csv;

public class CsvConnectorArgs : IConnectorArgs
{
    public required string FilePath { get; set; }
}