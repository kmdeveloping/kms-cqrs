using cqrsCore.Command;
using cqrsCore.Common;
using cqrsCore.Logging;
using Polly;

namespace cqrsCore.Decorators.Command;

/// <summary>
    /// Command Handler decorator for retrying a command based on the type of retry the command supports.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
public class RetryCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand: ICommand, IRetryable
    {
        private readonly ICommandHandler<TCommand> _decoratedHandler;
        private readonly ILogger _logger;

        public RetryCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler, ILogger logger)
        {
            _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            string commandName = command.GetType().GetFriendlyName();
            
            PolicyBuilder policyBuilder = Policy.Handle<Exception>();
            //Policy policy = Policy.NoOp();
            AsyncPolicy policy = Policy.NoOpAsync();

            if (command.RetrySettings != null && command.RetrySettings.Enabled)
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
                                    "Retrying command {Command} with exponential back-off due to exception {Exception}}, attempt {retryCount} of {retrySettings.Count} in {timeSpan.TotalSeconds} seconds...",
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
                                    "Retrying command {Command} due to exception {Exception}, attempt {RetryCount} of {MaxRetries} with delay {RetryDelay} seconds...",
                                    commandName, exception, retryCount, retrySettings.Count, retrySettings.Delay);
                            });
                }
                else
                {
                    policy = policyBuilder
                        .RetryAsync(retrySettings.Count, (exception, retryCount, context) =>
                        {
                            _logger.Warning(exception, 
                                "Retrying command {Command} due to exception {Exception}, attempt {RetryCount} of {MaxRetries}", 
                                commandName, exception, retryCount, retrySettings.Count);
                        });
                }
            }
            
            await policy.ExecuteAsync(async () => await _decoratedHandler.HandleAsync(command, cancellationToken));
        }
    }