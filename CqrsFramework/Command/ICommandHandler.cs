namespace CqrsFramework.Command;

/// <summary>
/// The contract for a command handler.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
  /// <summary>
  /// Executes the specified command.
  /// Any applied decorators will be executed first (such as validation, etc.).
  /// </summary>
  /// <param name="command">The command to execute.</param>
  /// <param name="cancellationToken"></param>
  Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}