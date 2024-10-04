using Microsoft.Extensions.Logging;

namespace DashboardTests.Utils;

internal record LogEntry
{
    public LogLevel LogLevel { get; init; }
    public EventId EventId { get; set; }
    public required string Message { get; init; }
    public Exception? Exception { get; set; }
}

public class ShimLogger<T> : ILogger<T>
{
    // Store a list of log entries, so we can assert on them later
    private readonly List<LogEntry> _logEntries = [];

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // Store the log entry
        _logEntries.Add(new LogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            Message = formatter(state, exception),
            Exception = exception
        });
    }

    public void AssertLog(LogLevel logLevel, string message)
    {
        if (_logEntries.Exists(e => e.LogLevel == logLevel && e.Message == message))
        {
            Assert.Pass();
        }
        else
        {
            var closestMatch = _logEntries
                .OrderBy(e => LevenshteinDistance(e.Message, message))
                .First();

            Assert.Fail(
                $"Expected log entry not found. Did you mean: [{closestMatch.LogLevel}] - '{closestMatch.Message}'?");
        }
    }

    /// <summary>
    ///     Find the Levenshtein distance between two strings.
    /// </summary>
    /// <param name="a">First string</param>
    /// <param name="b">Second string</param>
    /// <returns>The Levenshtein distance between a and b.</returns>
    private static int LevenshteinDistance(string a, string b)
    {
        var costs = new int[b.Length + 1];
        for (var i = 0; i <= a.Length; i++)
        {
            var lastValue = i;
            for (var j = 0; j <= b.Length; j++)
                if (i == 0)
                {
                    costs[j] = j;
                }
                else if (j > 0)
                {
                    var newValue = costs[j - 1];
                    if (a[i - 1] != b[j - 1])
                        newValue = Math.Min(Math.Min(newValue, lastValue), costs[j]) + 1;
                    costs[j - 1] = lastValue;
                    lastValue = newValue;
                }

            if (i > 0)
                costs[b.Length] = lastValue;
        }

        return costs[b.Length];
    }
}