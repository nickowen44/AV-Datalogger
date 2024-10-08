using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Dashboard.Utils;

public class RESConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)(value ?? "Not Activated") ? "Activated" : "Not Activated";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}