using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Query;
using CqrsFramework.Logging;
using CqrsFramework.RateLimiting;
using RateLimiter;

namespace CqrsFramework.Decorators.Query;

[DebuggerStepThrough]
public class RateLimitingQueryHandlerDecorator<TQuery,TResult> : IQueryHandler<TQuery,TResult>
    where TQuery: IQuery<TResult>
{
    private readonly IQueryHandler<TQuery,TResult> _decoratedHandler;
    private readonly ILogger _logger;
    private readonly RateLimiter.TimeLimiter? _rateLimiter;
    private readonly RateLimiterConstraints _constraints;
    private readonly Type _queryType = typeof(TQuery);

    public RateLimitingQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decoratedHandler,
        RateLimiterConstraints constraints, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(RateLimitingQueryHandlerDecorator<TQuery,TResult>));
        _constraints = constraints ?? throw new ArgumentNullException(nameof(constraints));

        if (IsQueryConfigured())
        {
            _rateLimiter = TimeLimiter.Compose(_constraints[_queryType].ToArray());
        }
    }

    private bool IsQueryConfigured()
    {
        return (_constraints.HasKey(_queryType));
    }

    /// <summary>
    /// Executes the specified query, applying any configured rate-limiting configuration.
    /// </summary>
    /// <param name="query">The query to execute</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The query result.</returns>
    /// <exception cref="ArgumentNullException">When query is null.</exception>
    public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        if (IsQueryConfigured() && _rateLimiter != null)
        {
            var queryName = query.GetType().GetFriendlyName();
            _logger.Debug("Executing query {QueryName} with rate-limiting", queryName);

            return await _rateLimiter.Enqueue(async () => 
                await _decoratedHandler.HandleAsync(query, cancellationToken), cancellationToken);
        }
        else
        {
            return await _decoratedHandler.HandleAsync(query, cancellationToken);
        }
    }
}