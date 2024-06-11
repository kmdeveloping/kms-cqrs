using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class LifetimeScopeCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
    private readonly Container _container;
    private readonly CommandHandlerOptions _commandHandlerOptions;

    public LifetimeScopeCommandHandlerProxy(Func<ICommandHandler<TCommand>> handlerFactory, Container container,
        CommandHandlerOptions commandHandlerOptions)
    {
        _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _commandHandlerOptions =
            commandHandlerOptions ?? throw new ArgumentNullException(nameof(commandHandlerOptions));
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        ScopedLifestyle? defaultScopedLifestyle =
            _container.Options.DefaultScopedLifestyle ?? new AsyncScopedLifestyle();
        var scope = _commandHandlerOptions.ReuseExistingScope
            ? defaultScopedLifestyle.GetCurrentScope(_container)
            : null;

        if (scope == null)
        {
            await using (_container.CreateLifetimeScope())
            {
                // NOTE: create the handler instance using the factory, so we can ensure
                //  the decoratee is resolved using its configured lifestyle, independent of this
                //  proxy class's lifestyle (which should be a singleton).
                await _handlerFactory().HandleAsync(command, cancellationToken);
            }
        }
        else
        {
            // NOTE: Reusing existing scope so objects with a scoped lifestyle will get the same instance during the lifetime of the scope
            await _handlerFactory().HandleAsync(command, cancellationToken);
        }
    }
}

public class CommandHandlerOptions
{ 
    public bool ReuseExistingScope { get; set; }
}