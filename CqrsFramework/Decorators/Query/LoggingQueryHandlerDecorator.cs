using System.Collections;
using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using CqrsFramework.Query;
using SimpleInjector;

namespace CqrsFramework.Decorators.Query;

    public class LoggingQueryHandlerDecorator<TQuery,TResult> : IQueryHandler<TQuery,TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery,TResult> _decoratedService;
        private readonly ILogger _logger;
        private readonly string _handlerName;
        
        public LoggingQueryHandlerDecorator(IQueryHandler<TQuery,TResult> decoratedService, ILogger logger, DecoratorContext decoratorContext)
        {
            _decoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (decoratorContext == null) throw new ArgumentNullException(nameof(decoratorContext));
            _handlerName = decoratorContext.ImplementationType.GetFriendlyName();
        }

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var queryName = query.GetType().GetFriendlyName();
            
            _logger.Debug("Handling query {Query}: {@QueryJson} using {QueryHandler}", 
                queryName, query, _handlerName);

            TResult result;
            var sw = Stopwatch.StartNew();
            try
            {
                result = await _decoratedService.HandleAsync(query, cancellationToken);
                sw.Stop();
                if (result is ICollection collection)
                    _logger.Debug("Handled query {Query} in {QueryExecutionTime} msec, returned {ResultCount} results", 
                        queryName, sw.ElapsedMilliseconds, collection.Count);    
                else
                    _logger.Debug("Handled query {Query} in {QueryExecutionTime} msec", 
                        queryName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Error(ex, "Failed handling query {Query} after {QueryExecutionTime} msec", 
                     queryName, sw.ElapsedMilliseconds);
                throw;
            }

            return result;
        }
    }