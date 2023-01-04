namespace cqrsCore.Logging;

public class LogEntry
{
  public readonly LoggingEventLevel Severity;
  public readonly string MessageTemplate;
  public object[] PropertyValues;
  public readonly Exception Exception;

  public LogEntry(LoggingEventLevel severity, string messageTemplate, Exception exception = null, params object[] propertyValues)
  {
    if(string.IsNullOrEmpty(messageTemplate)) throw new ArgumentNullException(nameof(messageTemplate));
            
    Severity = severity;
    MessageTemplate = messageTemplate;
    Exception = exception;
    PropertyValues = propertyValues;
  }
}