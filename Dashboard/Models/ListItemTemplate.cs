using System;

namespace Dashboard.Models;

public record ListItemTemplate(Type View, Type? ViewModel,  string Label);
