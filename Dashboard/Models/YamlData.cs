using System.Collections.Generic;

namespace Dashboard.Models;

/// <summary>
///     Class to represent structure of the yaml file.
/// </summary>
public record YamlData
{
    public required List<StepData> Steps { get; set; }
    public required string Top { get; set; }
    public required string Bottom { get; set; }
}