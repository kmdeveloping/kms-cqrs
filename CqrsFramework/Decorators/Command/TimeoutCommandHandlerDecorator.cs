using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using Polly;
using Polly.Timeout;

namespace CqrsFramework.Decorators.Command;

/// <summary>
/// Command handler decorator that applies a timeout to a command execution.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
[DebuggerStepThrough]
public class TimeoutCommandHandlerDecorator<TCommand>(ICommandHandler<TCommand> decoratedHandler, ILogger logger) : ICommandHandler<TCommand>
    where TCommand : ICommand, ITimeout
{
    private readonly ICommandHandler<TCommand> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    private readonly ILogger _logger = logger?.ForContext(typeof(TimeoutCommandHandlerDecorator<TCommand>)) ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        var commandName = command.GetType().GetFriendlyName();
            
        var timeout = command as ITimeout;
            
        await Policy.TimeoutAsync(timeout.TimeoutInSeconds, TimeoutStrategy.Pessimistic,
                (context, timeSpan, task, exception) =>
                {
                    _logger.Error(exception, "Command {CommandName} timed out (timeout = {TimeoutSeconds} seconds)", 
                        commandName, timeout.TimeoutInSeconds);
                    throw exception;
                })
            .ExecuteAsync(async () => await _decoratedHandler.HandleAsync(command, cancellationToken));
    }
}