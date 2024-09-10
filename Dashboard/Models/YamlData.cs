using System.Collections.Generic;

namespace Dashboard.Models;

/// <summary>
///     Class to represent structure of the yaml file
/// </summary>
public class YamlData : IYamlData
{
    public List<StepData> Steps { get; set; } = new();
    public string Top { get; set; } = string.Empty;
    public string Bottom { get; set; } = string.Empty;
}