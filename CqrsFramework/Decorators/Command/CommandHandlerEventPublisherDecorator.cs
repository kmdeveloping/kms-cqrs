using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Event;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class CommandHandlerEventPublisherDecorator<TCommand>(ICommandHandler<TCommand> decoratedHandler, IEventProcessor eventProcessor)
    : ICommandHandler<TCommand> where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    private readonly IEventProcessor _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));

    /// <inheritdoc />
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        await _eventProcessor.ProcessAsync(new OnBeforeCommandHandled<TCommand>(command), cancellationToken);
            
        await _decoratedHandler.HandleAsync(command, cancellationToken);
            
        await _eventProcessor.ProcessAsync(new OnAfterCommandHandled<TCommand>(command), cancellationToken);
    }
}