using System.Diagnostics;

namespace CqrsFramework.Logging;

[DebuggerStepThrough]
public class LogEntry
{
    public readonly LoggingEventLevel Severity;
    public readonly string MessageTemplate;
    public object[] PropertyValues;
    public readonly Exception Exception;

    public LogEntry(LoggingEventLevel severity, string messageTemplate, Exception exception = null,
        params object[] propertyValues)
    {
        if(string.IsNullOrEmpty(messageTemplate)) throw new ArgumentNullException(nameof(messageTemplate));
            
        this.Severity = severity;
        this.MessageTemplate = messageTemplate;
        this.Exception = exception;
        this.PropertyValues = propertyValues;
    }
}