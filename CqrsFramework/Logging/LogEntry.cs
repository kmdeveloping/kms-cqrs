using System.Diagnostics;

namespace CqrsFramework.Logging;

[DebuggerStepThrough]
public class LogEntry(LoggingEventLevel severity, string messageTemplate, Exception exception = null, params object[] propertyValues)
{
    public readonly LoggingEventLevel Severity = severity;
    public readonly string MessageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));
    public object[] PropertyValues = propertyValues;
    public readonly Exception Exception = exception;
}