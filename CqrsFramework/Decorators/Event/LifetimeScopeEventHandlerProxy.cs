using CqrsFramework.Events;
using CqrsFramework.Extensions;
using SimpleInjector;

namespace CqrsFramework.Decorators.Event;

public class LifetimeScopeEventHandlerProxy<TEvent> : IEventHandler<TEvent>
  where TEvent: IEvent
{
  private readonly Func<IEventHandler<TEvent>> _handlerFactory;
  private readonly Container _container;

  public LifetimeScopeEventHandlerProxy(Func<IEventHandler<TEvent>> handlerFactory, Container container)
  {
    _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    _container = container ?? throw new ArgumentNullException(nameof(container));
  }

  public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
  {
    if (@event == null) throw new ArgumentNullException(nameof(@event));

    using (_container.CreateLifetimeScope())
    {
      await _handlerFactory().HandleAsync(@event, cancellationToken);
    }
  }
}