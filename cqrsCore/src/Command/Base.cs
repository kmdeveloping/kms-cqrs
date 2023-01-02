namespace cqrsCore.Command;

public abstract class Base : ICommand
{
  /// <inheritdoc />
  public bool ExecuteAsNoOp { get; set; }
  
  /// <inheritdoc/>
  public IDictionary<string, object> ContextData { get; set; }
}