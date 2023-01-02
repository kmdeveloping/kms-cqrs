namespace cqrsCore;

public interface ICqrsManager
{
  /// <summary>
  /// Executes the specified command.
  /// </summary>
  /// <param name="command"></param>
  /// <param name="cancellationToken">Cancellation token.</param>
  Task ExecuteAsync(object command, CancellationToken cancellationToken = default); // todo change object to type ICommand
}

public class CqrsManager : ICqrsManager
{
  public CqrsManager()
  {
    
  }

  public async Task ExecuteAsync(object command, CancellationToken cancellationToken = default)
  {
    await Task.CompletedTask; // todo update with command
  }
}