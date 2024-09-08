using System.Collections.Generic;

namespace Dashboard.Models;

public interface IStepData
{
    public string Step { get; }
    public List<string> Measurements { get; }
}