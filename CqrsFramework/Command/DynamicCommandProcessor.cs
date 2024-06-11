using System.Diagnostics;
using CqrsFramework.Exceptions;
using SimpleInjector;

namespace CqrsFramework.Command;

[DebuggerStepThrough]
public class DynamicCommandProcessor : ICommandProcessor
{
    private readonly Func<Type, object> _handlerFactory;

    public DynamicCommandProcessor(Container container)
    {
        if(container == null) throw new ArgumentNullException(nameof(container));
        _handlerFactory = container.GetInstance;
    }

    public async Task ProcessAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        dynamic handler = _handlerFactory.Invoke(handlerType);
        if (handler == null)
            throw new DependencyNotFoundException(handlerType);

        await handler.HandleAsync((dynamic) command, cancellationToken);
    }
}