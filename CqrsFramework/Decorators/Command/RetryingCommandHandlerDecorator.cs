using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using Polly;

namespace CqrsFramework.Decorators.Command;

/// <summary>
/// Command Handler decorator for retrying a command based on the type of retry the command supports.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
[DebuggerStepThrough]
public class RetryingCommandHandlerDecorator<TCommand>(ICommandHandler<TCommand> decoratedHandler, ILogger logger) : ICommandHandler<TCommand> where TCommand : ICommand, IRetryable
{
    private readonly ICommandHandler<TCommand> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    private readonly ILogger _logger = logger?.ForContext(typeof(RetryingCommandHandlerDecorator<TCommand>)) ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var commandName = command.GetType().GetFriendlyName();
            
        var policyBuilder = Policy.Handle<Exception>();
        //Policy policy = Policy.NoOp();
        AsyncPolicy policy = Policy.NoOpAsync();

        if (command.RetrySettings is { Enabled: true })
        {
            var retryableCommand = (command as IRetryable);
            var retrySettings = retryableCommand.RetrySettings;

            if (retrySettings.BackoffPower > 1)
            {
                policy = policyBuilder
                    .WaitAndRetryAsync(retrySettings.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySettings.BackoffPower, retryAttempt)),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.Warning(exception,
                                "Retrying command {CommandName} with exponential back-off due to exception {Exception}, attempt {RetryCount} of {MaxRetries} in {TotalSeconds} seconds",
                                commandName, exception, retryCount, retrySettings.Count, timeSpan.TotalSeconds);
                        });
            }
            else if (retryableCommand.RetrySettings.Delay > 0)
            {
                policy = policyBuilder
                    .WaitAndRetryAsync(retrySettings.Count,
                        retryAttempt => TimeSpan.FromSeconds(retrySettings.Delay),
                        (exception, TimeSpan, retryCount, context) =>
                        {
                            _logger.Warning(exception,
                                "Retrying command {CommandName} due to exception {Exception}, attempt {RetryCount} of {MaxRetries} with delay {RetryDelay} seconds",
                                commandName, exception, retryCount, retrySettings.Count, retrySettings.Delay);
                        });
            }
            else
            {
                policy = policyBuilder
                    .RetryAsync(retrySettings.Count, (exception, retryCount, context) =>
                    {
                        _logger.Warning(exception, 
                            "Retrying command {CommandName} due to exception {Exception}, attempt {RetryCount} of {MaxRetries}", 
                            commandName, exception, retryCount, retrySettings.Count);
                    });
            }
        }
            
        await policy.ExecuteAsync(() => _decoratedHandler.HandleAsync(command, cancellationToken));
    }
}