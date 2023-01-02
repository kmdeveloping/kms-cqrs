using cqrsCore.Command;

namespace cqrsCore.Decorators.Command;

public class AsyncCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
  private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

  public AsyncCommandHandlerDecorator(Func<ICommandHandler<TCommand>> handlerFactory)
  {
    _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
  }
  
  /// <summary>
  /// Executes the specified command.
  /// Any applied decorators will be executed first (such as validation, etc.).
  /// </summary>
  /// <param name="command"></param>
  /// <param name="cancellationToken"></param>
  public Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
  {
    return Task.Run(async () =>
    {
      ICommandHandler<TCommand> handler = _handlerFactory.Invoke();
      await handler.HandleAsync(command, cancellationToken);
    }, cancellationToken);
  }
}