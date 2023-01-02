using cqrsCore.Exceptions;
using SimpleInjector;

namespace cqrsCore.Command;

public class DynamicCommandProcessor : ICommandProcessor
{
  private readonly Func<Type, object> _handlerFactory;

  public DynamicCommandProcessor(Container container)
  {
    if (container is null) throw new ArgumentNullException(nameof(container));
    _handlerFactory = container.GetInstance;
  }

  public async Task ProcessAsync(ICommand command, CancellationToken cancellationToken = default)
  {
    var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

    dynamic handler = _handlerFactory.Invoke(handlerType);

    if (handler is null) throw new DependencyNotFoundException(handlerType);

    await handler.HandleAsync((dynamic) command, cancellationToken);
  }
}