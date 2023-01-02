using cqrsCore.Command;

namespace cqrsCore;

public interface ICqrsManager
{
  /// <summary>
  /// Executes the specified command.
  /// </summary>
  /// <param name="command"></param>
  /// <param name="cancellationToken">Cancellation token.</param>
  Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);
}

public class CqrsManager : ICqrsManager
{
  private readonly ICommandProcessor _commandProcessor;
  
  public CqrsManager(ICommandProcessor commandProcessor)
  {
    _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
  }
  
  /// <inheritdoc />
  public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
  {
    await _commandProcessor.ProcessAsync(command, cancellationToken);
  }
}