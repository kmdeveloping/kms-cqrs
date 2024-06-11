using CqrsFramework.Events;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Event;

public class ValidatingEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
  where TEvent: IEvent
{
  private readonly IEventHandler<TEvent> _decoratedHandler;
  private readonly IValidator _validator;
        
  public ValidatingEventHandlerDecorator(IEventHandler<TEvent> decoratedHandler, IValidator validator)
  {
    _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
  }

  public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
  {
    if (@event == null) throw new ArgumentNullException(nameof(@event));

    await _validator.ValidateAsync(@event, cancellationToken);
    await _decoratedHandler.HandleAsync(@event, cancellationToken);
  }
}