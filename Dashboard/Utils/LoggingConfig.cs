using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Dashboard.Utils;

public static class LoggingConfig
{
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
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/dashboard.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();
    }
}