namespace CqrsFramework.Logging;

public interface ILogger
{
  void Log(LogEntry entry);
  IDisposable PushProperty(string name, object value, bool destructureObjects = false);
}

public interface ILogger<T> : ILogger
{
}