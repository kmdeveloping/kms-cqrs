namespace cqrsCore.Events;

public class EventBase : IEvent
{      
  /// <inheritdoc />
  public bool ExecuteAsNoOp { get; set; }

  /// <inheritdoc />
  public IDictionary<string, object> ContextData { get; set; }
  
}