using System.Collections.Generic;

namespace Dashboard.Models;

public class IYamlData
{
    public List<StepData> Steps { get; set; }
    public string Top { get; set; }
    public string Bottom { get; set; }
}