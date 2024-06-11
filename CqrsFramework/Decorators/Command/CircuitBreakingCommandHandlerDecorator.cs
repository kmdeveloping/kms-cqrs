using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using Polly;

namespace CqrsFramework.Decorators.Command;

/// <summary>
/// Command handler decorator that implements a circuit breaker pattern for a command handler.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
[DebuggerStepThrough]
public class CircuitBreakingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand, ICircuitBreakable
{
    private readonly ICommandHandler<TCommand> _decoratedHandler;
    private readonly ILogger _logger;

    public CircuitBreakingCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(CircuitBreakingCommandHandlerDecorator<TCommand>));
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        if (command.CircuitBreakerSettings == null) throw new ArgumentNullException(nameof(command.CircuitBreakerSettings));
        
        string commandName = command.GetType().GetFriendlyName();
        AsyncPolicy policy = Policy.NoOpAsync();
        
        PolicyBuilder? policyBuilder = Policy.Handle<Exception>();
        if (command.CircuitBreakerSettings.ExceptionPredicates != null && command.CircuitBreakerSettings.ExceptionPredicates.Any())
        {
            foreach (var predicate in command.CircuitBreakerSettings.ExceptionPredicates)
            {
                policyBuilder = policyBuilder.Or(predicate);
            }
        }

        if (command.CircuitBreakerSettings != null && command.CircuitBreakerSettings.Enabled)
        {
            policy = policyBuilder
                .CircuitBreakerAsync(command.CircuitBreakerSettings.ExceptionsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(command.CircuitBreakerSettings.DurationOfBreakInSeconds),
                    onBreak: (exception, timespan) =>
                    {
                        _logger.Warning(exception,
                            "Circuit breaker opened for command {CommandName} due to exception {Exception}, breaking for {TotalSeconds} seconds",
                            commandName, exception, timespan.TotalSeconds);
                    },
                    onReset: () =>
                    {
                        _logger.Information("Circuit breaker reset for command {CommandName}", commandName);
                    },
                    onHalfOpen: () =>
                    {
                        _logger.Information("Circuit breaker half-opened for command {CommandName}", commandName);
                    });
        }
        
        await policy.ExecuteAsync(() => _decoratedHandler.HandleAsync(command, cancellationToken));
    }
}