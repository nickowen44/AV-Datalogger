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

public class MeasurementValueVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Assuming `parameter` holds the text from the data context to compare with
        var comparisonText = parameter as string;
        var boundText = value as string;
        
        Console.WriteLine("1 " + comparisonText );
        Console.WriteLine("2 " + boundText );

    
        return string.Equals(boundText.Replace(" ", ""), comparisonText, StringComparison.OrdinalIgnoreCase);
    }

    // ConvertBack is not used but we need it here to correctly implement the value converter interface
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}