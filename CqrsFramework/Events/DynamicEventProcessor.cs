using CqrsFramework.Exceptions;
using SimpleInjector;

namespace CqrsFramework.Events;

public class DynamicEventProcessor : IEventProcessor
{
  private readonly Func<Type, object> _handlerFactory;

  public DynamicEventProcessor(Container container)
  {
    if(container == null) throw new ArgumentNullException(nameof(container));
    _handlerFactory = container.GetInstance;
  }

  public async Task ProcessAsync(IEvent @event, CancellationToken cancellationToken = default)
  {
    var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
    dynamic handler = _handlerFactory.Invoke(handlerType);
    if(handler == null)
      throw new DependencyNotFoundException(handlerType);

    await handler.HandleAsync((dynamic)@event, cancellationToken);
  }
}