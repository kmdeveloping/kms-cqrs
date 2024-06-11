using CqrsFramework.Extensions;
using CqrsFramework.Query;
using SimpleInjector;

namespace CqrsFramework.Decorators.Query;

public class LifetimeScopeQueryHandlerProxy<TQuery,TResult> : IQueryHandler<TQuery, TResult>
  where TQuery: IQuery<TResult>
{
  private readonly Func<IQueryHandler<TQuery,TResult>> _handlerFactory;
  private readonly Container _container;

  public LifetimeScopeQueryHandlerProxy(Func<IQueryHandler<TQuery, TResult>> handlerFactory, Container container)
  {
    _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    _container = container ?? throw new ArgumentNullException(nameof(container));
  }

  public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default)
  {
    if (query == null) throw new ArgumentNullException(nameof(query));

    // NOTE: Re-use the current scope, if it exists
    if (!_container.ScopeExists())
    {
      using (_container.CreateLifetimeScope())
      {
        // NOTE: create the handler instance inside the scope, so we can ensure
        //  the decoratee is resolved using its configured lifestyle, independent of this
        //  proxy class's lifestyle (which should be a singleton).
        return await _handlerFactory().HandleAsync(query, cancellationToken);
      }
    }
    else
    {
      // Scope already exists, so just execute without creating new scope
      return await _handlerFactory().HandleAsync(query, cancellationToken);
    }
  }
}