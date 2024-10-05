using System.Collections.Generic;

namespace Dashboard.Models;

/// <summary>
///     The data class for each step of the flowchart.
/// </summary>
public record StepData
{
    public required string Step { get; set; }
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Caution { get; set; }
    public required List<string> Measurements { get; set; }
}