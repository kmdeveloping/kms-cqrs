using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using CqrsFramework.RateLimiting;
using RateLimiter;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class RateLimitingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand: ICommand
{
    private readonly ICommandHandler<TCommand> _decoratedHandler;
    private readonly ILogger _logger;
    private readonly TimeLimiter? _rateLimiter;
    private readonly RateLimiterConstraints _constraints;
    private readonly Type _commandType = typeof(TCommand);

    public RateLimitingCommandHandlerDecorator(ILogger logger, RateLimiterConstraints constraints,
        ICommandHandler<TCommand> decoratedHandler)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if(logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(RateLimitingCommandHandlerDecorator<TCommand>));
        _constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));
            
        if (IsCommandConfigured())
            _rateLimiter = TimeLimiter.Compose(_constraints[_commandType].ToArray());
    }

    private bool IsCommandConfigured()
    {
        return (_constraints.HasKey(_commandType));
    }

    /// <summary>
    /// Executes the specified command.
    /// Any applied decorators will be executed first (such as validation, etc.).
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken"></param>
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        // NOTE: Only use rate limiter for commands which are configured...
        if (IsCommandConfigured() && _rateLimiter != null)
        {
            string commandName = command.GetType().GetFriendlyName();
            _logger.Debug("Executing command {CommandName} with rate-limiting", commandName);
                
            await _rateLimiter.Enqueue(async () => 
                await _decoratedHandler.HandleAsync(command, cancellationToken), cancellationToken);
        }
        else
        {
            await _decoratedHandler.HandleAsync(command, cancellationToken);
        }
    }
}