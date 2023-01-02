using Serilog.Context;

namespace cqrsCore.Logging;

public class SerilogAdapter<T> : ILogger<T>
{
  private readonly Serilog.ILogger _serilogLogger;

  public SerilogAdapter(Serilog.ILogger serilogLogger)
  {
    if(serilogLogger == null) throw new ArgumentNullException(nameof(serilogLogger));

    _serilogLogger = serilogLogger.ForContext<T>();
  }
  
  public void Log(LogEntry entry)
  {
    if (entry.Severity == LoggingEventLevel.Verbose)
      _serilogLogger.Verbose(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Debug)
      _serilogLogger.Debug(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Information)
      _serilogLogger.Information(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Warning)
      _serilogLogger.Warning(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Error)
      _serilogLogger.Error(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else
      _serilogLogger.Fatal(entry.Exception, entry.MessageTemplate, entry.PropertyValues);
  }

  public IDisposable PushProperty(string name, object value, bool destructureObjects = false)
  {
    return LogContext.PushProperty(name, value, destructureObjects);
  }
}

public class SerilogAdapter : ILogger
{
  private readonly Serilog.ILogger _serilogLogger;

  public SerilogAdapter(Serilog.ILogger serilogLogger)
  {
    _serilogLogger = serilogLogger ?? throw new ArgumentNullException(nameof(serilogLogger));
  }
        
  public void Log(LogEntry entry)
  {
    if (entry.Severity == LoggingEventLevel.Verbose)
      _serilogLogger.Verbose(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Debug)
      _serilogLogger.Debug(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Information)
      _serilogLogger.Information(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Warning)
      _serilogLogger.Warning(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else if (entry.Severity == LoggingEventLevel.Error)
      _serilogLogger.Error(entry.Exception, entry.MessageTemplate, entry.PropertyValues);

    else
      _serilogLogger.Fatal(entry.Exception, entry.MessageTemplate, entry.PropertyValues);
  }

  public IDisposable PushProperty(string name, object value, bool destructureObjects = false)
  {
    return LogContext.PushProperty(name, value, destructureObjects);
  }
}