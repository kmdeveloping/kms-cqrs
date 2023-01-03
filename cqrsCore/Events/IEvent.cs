namespace cqrsCore.Events;

public interface IEvent
{
  /// <summary>
  /// If set to true, event will execute as no-op and will not mutate state in the system.
  /// </summary>
  bool ExecuteAsNoOp { get; set; }
        
  /// <summary>
  /// Optional contextual data which can be attached to events.
  /// </summary>
  IDictionary<string,object> ContextData { get; set; }
}