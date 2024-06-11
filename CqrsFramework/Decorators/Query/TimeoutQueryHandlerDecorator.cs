using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using CqrsFramework.Query;
using Polly;
using Polly.Timeout;

namespace CqrsFramework.Decorators.Query;

/// <summary>
/// Query handler decorator that applies a timeout to query execution.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
[DebuggerStepThrough]
public class TimeoutQueryHandlerDecorator<TQuery,TResult> : IQueryHandler<TQuery,TResult>
    where TQuery: IQuery<TResult>, ITimeout
{
    private readonly IQueryHandler<TQuery,TResult> _decoratedHandler;
    private readonly ILogger _logger;

    public TimeoutQueryHandlerDecorator(IQueryHandler<TQuery,TResult> decoratedHandler, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(TimeoutQueryHandlerDecorator<TQuery,TResult>));
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        string queryName = query.GetType().GetFriendlyName();
        var timeout = query as ITimeout;

        TResult result = await Policy.TimeoutAsync<TResult>(timeout.TimeoutInSeconds, TimeoutStrategy.Pessimistic,
                (context, timeSpan, task, exception) =>
                {
                    _logger.Error(exception, "Query {QueryName} timed out (timeout = {Timeout} seconds)", queryName, timeout.TimeoutInSeconds);
                    throw exception;
                })
            .ExecuteAsync(async (ct) => 
                await _decoratedHandler.HandleAsync(query, ct), cancellationToken);

        return result;
    }
}