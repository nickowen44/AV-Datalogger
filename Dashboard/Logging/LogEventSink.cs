using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace Dashboard.Logging;

public class LogEventSink : ILogEventSink
{
    public event EventHandler<string>? LogMessageReceived;

    private readonly MessageTemplateTextFormatter _formatter = new(
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

    public void Emit(LogEvent logEvent)
    {
        using var formatter = new StringWriter();
        _formatter.Format(logEvent, formatter);

        LogMessageReceived?.Invoke(this, formatter.ToString());
    }
}