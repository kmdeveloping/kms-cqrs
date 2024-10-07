using System.Diagnostics;

namespace CqrsFramework.Event;

[DebuggerStepThrough]
public abstract class EventBase : IEvent
{
    /// <inheritdoc />
    public DateTime? TimeUtc { get; set; } = DateTime.UtcNow;
    /// <inheritdoc />
    public string? Source { get; set; }
    /// <inheritdoc />
    public IDictionary<string, string>? Metadata { get; set; } = new Dictionary<string, string>();
}