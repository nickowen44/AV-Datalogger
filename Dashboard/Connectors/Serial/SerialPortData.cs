namespace Dashboard.Connectors.Serial;

public record SerialPortData
{
    public required string Buffer { get; init; }
}