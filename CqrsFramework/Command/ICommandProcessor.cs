namespace CqrsFramework.Command;

public interface ICommandProcessor
{
    /// <summary>
    /// Processes the specified command.
    /// </summary>
    /// <param name="command">The command to process.</param>
    /// <param name="cancellationToken"></param>
    Task ProcessAsync(ICommand command, CancellationToken  cancellationToken = default);
}