using System.Collections.Generic;

namespace Dashboard.Models;

public interface IStepData
{
    public string Step { get; set; }
    public List<string> Measurements { get; set; }
}