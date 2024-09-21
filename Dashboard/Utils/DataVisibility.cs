using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Dashboard.Utils;

/// <summary>
///     Determines whether "DV Data" header is displayed on the frontend of the scrutineering page.
///     Conditional rendering in avalonia essentially.
/// </summary>
public class DvDataVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is List<string> { Count: > 0 };
    }

    // ConvertBack is not used but we need it here to correctly implement the value converter interface
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
///     Determines which of the variables for the scrutineering page should be shown depending on which measurment item is
///     being displayed on the current carousel slide.
/// </summary>
public class MeasurementValueVisibility : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values[0] switch
        {
            "Steering Angle" => $"Steering Angle: {values[1]}",
            "Autonomous Mission Indicator" => $"Autonomous Mission Indicator: {values[2]}",
            "Autonomous System State" => $"Autonomous System State: {values[3]}",
            "Service Brake State" => $"Service Brake State Enabled: {values[4]}",
            "Emergency Brake State" => $"Emergency Brake State: {values[5]}",
            _ => "" // Fallback to the empty text if it's neither
        };
    }
}