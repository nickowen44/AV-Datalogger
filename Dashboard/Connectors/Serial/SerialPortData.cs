namespace Dashboard.Connectors.Serial;

public record SerialPortData
{
    public string Buffer { get; init; } = string.Empty;
}