﻿using System.Collections.Generic;

namespace Dashboard.Models;

/// <summary>
///     The data class for each step of the flowchart.
/// </summary>
public class StepData : IStepData
{
    public string Step { get; set; }
    public List<string> Measurements { get; set; }
}