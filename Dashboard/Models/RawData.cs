namespace Dashboard.Models;

public record RawData
{
    public string CarId { get; init; }
    public string UTCTime { get; init; }
    public string RawMessage { get; init; }
}