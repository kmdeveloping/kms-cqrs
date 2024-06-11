using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using CqrsFramework.Query;
using Polly;

namespace CqrsFramework.Decorators.Query;

/// <summary>
/// Query Handler decorator for retrying a query on the type of retry the query supports.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResult">The type the query returns.</typeparam>
[DebuggerStepThrough]
public class RetryingQueryHandlerDecorator<TQuery,TResult> : IQueryHandler<TQuery,TResult>
    where TQuery: IQuery<TResult>, IRetryable
{
    private readonly IQueryHandler<TQuery,TResult> _decoratedHandler;
    private readonly ILogger _logger;
        
    public RetryingQueryHandlerDecorator(IQueryHandler<TQuery,TResult> decoratedHandler, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(RetryingQueryHandlerDecorator<TQuery,TResult>));
    }
        
    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));
            
        string queryName = query.GetType().GetFriendlyName();
        TResult result = default(TResult);

        PolicyBuilder policyBuilder = Policy.Handle<Exception>();
        AsyncPolicy policy = Policy.NoOpAsync();

        if (query.RetrySettings != null && query.RetrySettings.Enabled)
        {
            var retryable = (query as IRetryable);
            var retrySettings = retryable.RetrySettings;

            if (retrySettings.BackoffPower > 1)
            {
                policy = policyBuilder
                    .WaitAndRetryAsync(retrySettings.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySettings.BackoffPower, retryAttempt)),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.Warning(exception,
                                "Retrying query {QueryName} with exponential back-off due to exception {Exception}, attempt {RetryAttempt} of {MaxRetries} in {RetryInterval} seconds",
                                queryName, exception, retryCount, retrySettings.Count, timeSpan.TotalSeconds);
                        });
            }
            else if (retryable.RetrySettings.Delay > 0)
            {
                policy = policyBuilder
                    .WaitAndRetryAsync(retrySettings.Count,
                        retryAttempt => TimeSpan.FromSeconds(retrySettings.Delay),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.Warning(exception,
                                "Retrying query {QueryName} due to exception {Exception}, attempt {RetryAttempt} of {MaxRetries} with delay {RetryInterval} seconds",
                                queryName, exception, retryCount, retrySettings.Count, retrySettings.Delay);
                        });
            }
            else
            {
                policy = policyBuilder
                    .RetryAsync(retrySettings.Count, (exception, retryCount, context) =>
                    {
                        _logger.Warning(exception, "Retrying query {QueryName} due to exception {Exception}, attempt {RetryAttempt} of {MaxRetries}", 
                            queryName, exception, retryCount, retrySettings.Count);
                    });
            }
        }
            
        await policy.ExecuteAsync(async (ct) => 
            result = await _decoratedHandler.HandleAsync(query, ct), cancellationToken);

        return result;
    }
}