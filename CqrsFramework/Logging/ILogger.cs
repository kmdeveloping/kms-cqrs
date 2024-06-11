namespace CqrsFramework.Logging
{
    public interface ILogger
    {
        void Log(LogEntry entry);
        IDisposable PushProperty(string name, object value, bool destructureObjects = false);
        ILogger ForContext(Type source);
        ILogger ForContext<TContext>();
        ILogger ForContext(string propertyName, object? value, bool destructureObjects = false);
    }

    public interface ILogger<T> : ILogger
    {
    }
}