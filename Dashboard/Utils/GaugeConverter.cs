using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Dashboard.Utils;

public class GaugeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var inputValue = value is double d ? d : 0;

        // If this cast fails, it doesnt matter as we will be setting the needle to 0
        var type = parameter as string ?? "BrakeMeter";

        // Check if the convertee is the Speedometer or Brake meter, Defaults to Brake meter.
        double maxValue = 100;
        if (type == "Speedometer")
        {
            maxValue = 120;
        }

        // Center of the gauge.
        double centerX = 200;
        double centerY = 250;
        double needleLength = 70;

        // Convert input to angle (from 0-180).
        double angle = 180 - (inputValue / maxValue) * 180;

        double radians = Math.PI * angle / 180.0;
        double endX = centerX + needleLength * Math.Cos(radians);
        double endY = centerY - needleLength * Math.Sin(radians);

        // Return the line path for the needle
        return $"M {centerX},{centerY} L {endX},{endY}";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}