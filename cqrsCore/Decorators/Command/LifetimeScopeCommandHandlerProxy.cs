using cqrsCore.Command;
using cqrsCore.Extensions;
using SimpleInjector;

namespace cqrsCore.Decorators.Command;

public class LifetimeScopeCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
  where TCommand : ICommand
{
  private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
  private readonly Container _container;

  public LifetimeScopeCommandHandlerProxy(Func<ICommandHandler<TCommand>> handlerFactory, Container container)
  {
    _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    _container = container ?? throw new ArgumentNullException(nameof(container));
  }

  public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
  {
    if (command == null) throw new ArgumentNullException(nameof(command));

    using (_container.CreateLifetimeScope())
    {
      // NOTE: create the handler instance using the factory, so we can ensure
      //  the decoratee is resolved using its configured lifestyle, independent of this
      //  proxy class's lifestyle (which should be a singleton).
      await _handlerFactory().HandleAsync(command, cancellationToken);
    }
  }
}