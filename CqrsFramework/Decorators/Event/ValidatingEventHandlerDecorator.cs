using System.Diagnostics;
using CqrsFramework.Event;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Event;

[DebuggerStepThrough]
public class ValidatingEventHandlerDecorator<TEvent>(IEventHandler<TEvent> decoratedHandler, IValidator validator) : IEventHandler<TEvent>
    where TEvent : IEvent
{
    private readonly IEventHandler<TEvent> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    private readonly IValidator _validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));

        await _validator.ValidateAsync(@event, cancellationToken);
        await _decoratedHandler.HandleAsync(@event, cancellationToken);
    }
}