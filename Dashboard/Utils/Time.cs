using System;
using System.Globalization;

namespace Dashboard.Utils;

public static class Time
{
    /// <summary>
    ///     Parses a Nuvotion AV-Logger UTC time string into a <see cref="DateTime" />.
    /// </summary>
    /// <param name="timeString"></param>
    /// <returns></returns>
    public static DateTime ParseUtcTime(string timeString)
    {
        // Check if the UTC value includes the leading zero for months, if not add.
        var datePart = timeString.Length switch
        {
            20 => timeString.Substring(1, 4) + "0" + timeString.Substring(5, 3) + timeString.Substring(9, 8),
            _ => timeString.Substring(1, 8) + timeString.Substring(9, 8)
        };

        var parsedDate = DateTime.ParseExact(datePart, @"yyyyMMddhh\:mm\:ss", CultureInfo.InvariantCulture);

        return parsedDate;
    }
}