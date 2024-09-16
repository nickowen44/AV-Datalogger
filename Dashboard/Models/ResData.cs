namespace Dashboard.Models;

public record ResData
{
    public bool ResState { get; init; }
    public bool K2State { get; init; }
    public bool K3State { get; init; }
    public int ResRadioQuality { get; init; }
    public int ResNodeId { get; init; }
}