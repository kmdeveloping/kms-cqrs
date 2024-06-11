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
public class TimeoutCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand: ICommand, ITimeout
{
    private readonly ICommandHandler<TCommand> _decoratedHandler;
    private readonly ILogger _logger;

    public TimeoutCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if(logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(TimeoutCommandHandlerDecorator<TCommand>));
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        string commandName = command.GetType().GetFriendlyName();
            
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