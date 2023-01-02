namespace cqrsCore.Command;

/// <summary>
/// Represents a command.
/// </summary>
public interface ICommand
{
  /// <summary>
  /// If set to true, command will execute as no-op and will not mutate state in the system.
  /// Handling of this is up to the command handler implementation to ensure state is not mutated.
  /// </summary>
  bool ExecuteAsNoOp { get; set; }

  /// <summary>
  /// Optional contextual data which can be attached to commands.
  /// </summary>
  IDictionary<string,object> ContextData { get; set; }
}

/// <summary>
/// Represents a command that returns a result.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ICommandWithResult<TResult> : ICommand
{
  /// <summary>
  /// The result of the command after execution.
  /// </summary>
  TResult Result { get; set; }
}