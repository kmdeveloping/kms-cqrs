using cqrsCore.Command;

namespace cqrsCore.Examples.ExampleCommand;

public class SayHelloCommand : ICommand
{
  public string Name { get; set; }
  
  /// <summary>
  /// If set to true, command will execute as no-op and will not mutate state in the system.
  /// Handling of this is up to the command handler implementation to ensure state is not mutated.
  /// </summary>
  public bool ExecuteAsNoOp { get; set; }
  
  /// <summary>
  /// Optional contextual data which can be attached to commands.
  /// </summary>
  public IDictionary<string, object> ContextData { get; set; }
}