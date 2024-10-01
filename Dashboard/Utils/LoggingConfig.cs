using Dashboard.Logging;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Dashboard.Utils;

public static class LoggingConfig
{
    public static LogEventSink LogEventSink { get; } = new();

    /// <summary>
    ///     Extends the <see cref="ILoggingBuilder" /> to add a configured Serilog logger.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to extend.</param>
    /// <returns>The <see cref="ILoggingBuilder" /> with the configured Serilog logger.</returns>
    public static void AddLogger(this ILoggingBuilder builder)
    {
        builder.AddSerilog(CreateLogger());
    }

    /// <summary>
    ///     Configures the logger for the application.
    /// </summary>
    /// <returns>A configured <see cref="Logger" />.</returns>
    private static Logger CreateLogger()
    {
        var logLevel = LogEventLevel.Information;
        // Set the log level to Debug if the DEBUG symbol is defined (during development).
#if DEBUG
        logLevel = LogEventLevel.Debug;
#endif

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/dashboard.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console(
                logLevel,
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Sink(LogEventSink, LogEventLevel.Information)
            .CreateLogger();
    }
}