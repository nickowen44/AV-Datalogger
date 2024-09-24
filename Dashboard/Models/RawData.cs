using System;

namespace Dashboard.Models;

public record RawData
{
    public string CarId { get; init; }
    public DateTime UTCTime { get; init; }
    public string RawMessage { get; init; }
    public bool ConnectionStatus { get; init; }
}