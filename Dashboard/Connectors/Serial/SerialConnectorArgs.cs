namespace Dashboard.Connectors.Serial;

public class SerialConnectorArgs : IConnectorArgs
{
    public string PortName { get; set; } = "COM22";
    public bool SaveToCsv { get; set; } = false;
}