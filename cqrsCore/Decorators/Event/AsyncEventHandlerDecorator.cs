using cqrsCore.Events;

namespace cqrsCore.Decorators.Event;

public class AsyncEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
  where TEvent: IEvent
{
  private readonly Func<IEventHandler<TEvent>> _handlerFactory;

  public AsyncEventHandlerDecorator(Func<IEventHandler<TEvent>> handlerFactory)
  {
    _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
  }

  public Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
  {
    return Task.Run(async () =>
    {
      // Create new handler in this thread...
      IEventHandler<TEvent> handler = _handlerFactory.Invoke();
      await handler.HandleAsync(@event, cancellationToken);
    }, cancellationToken);
  }
}