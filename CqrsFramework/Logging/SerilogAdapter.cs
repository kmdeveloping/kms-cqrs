using System.Diagnostics;
using Serilog.Context;

namespace CqrsFramework.Logging;

[DebuggerStepThrough]
public class SerilogAdapter<T>(Serilog.ILogger serilogLogger) : ILogger<T>
{
    private readonly Serilog.ILogger _serilogLogger = serilogLogger.ForContext<T>() ?? throw new ArgumentNullException(nameof(serilogLogger));
    
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

    public IDisposable PushProperty(string name, object value, bool destructureObjects = false) => LogContext.PushProperty(name, value, destructureObjects);
    
    public ILogger ForContext<TContext>()
    {
        // ReSharper disable once ContextualLoggerProblem
        var logger = _serilogLogger.ForContext<TContext>();
        return new SerilogAdapter(logger);
    }

    public ILogger ForContext(Type source)
    {
        var logger = _serilogLogger.ForContext(source);
        return new SerilogAdapter(logger);
    }

    public ILogger ForContext(string propertyName, object? value, bool destructureObjects = false)
    {
        var logger = _serilogLogger.ForContext(propertyName, value, destructureObjects);
        return new SerilogAdapter(logger);
    }
}

[DebuggerStepThrough]
public class SerilogAdapter(Serilog.ILogger serilogLogger) : ILogger
{
    private readonly Serilog.ILogger _serilogLogger = serilogLogger ?? throw new ArgumentNullException(nameof(serilogLogger));
        
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

    public IDisposable PushProperty(string name, object value, bool destructureObjects = false) => LogContext.PushProperty(name, value, destructureObjects);

    public ILogger ForContext(Type source)
    {
        var logger = _serilogLogger.ForContext(source);
        return new SerilogAdapter(logger);
    }
    
    public ILogger ForContext<TContext>()
    {
        // ReSharper disable once ContextualLoggerProblem
        var logger = _serilogLogger.ForContext<TContext>();
        return new SerilogAdapter(logger);
    }

    public ILogger ForContext(string propertyName, object? value, bool destructureObjects = false)
    {
        var logger = _serilogLogger.ForContext(propertyName, value, destructureObjects);
        return new SerilogAdapter(logger);
    }
}