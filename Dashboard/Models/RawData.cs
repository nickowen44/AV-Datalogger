using System;

namespace Dashboard.Models;

public record RawData
{
    public required string CarId { get; init; }
    public required DateTime UTCTime { get; init; }
    public required string RawMessage { get; init; }
    public bool ConnectionStatus { get; init; }
}