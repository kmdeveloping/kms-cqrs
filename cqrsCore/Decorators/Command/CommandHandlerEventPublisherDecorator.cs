using cqrsCore.Command;
using cqrsCore.Events;

namespace cqrsCore.Decorators.Command;

public class CommandHandlerEventPublisherDecorator<TCommand> : ICommandHandler<TCommand>
  where TCommand: ICommand
{
  private readonly ICommandHandler<TCommand> _decoratedHandler;
  private readonly IEventProcessor _eventProcessor;

  public CommandHandlerEventPublisherDecorator(ICommandHandler<TCommand> decoratedHandler, IEventProcessor eventProcessor)
  {
    _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
  }

  /// <inheritdoc />
  public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
  {
    await _eventProcessor.ProcessAsync(new OnBeforeCommandHandled<TCommand>(command), cancellationToken);
            
    await _decoratedHandler.HandleAsync(command, cancellationToken);
            
    await _eventProcessor.ProcessAsync(new OnAfterCommandHandled<TCommand>(command), cancellationToken);
  }
}