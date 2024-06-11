using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class AuthorizationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand: ICommand
{
    private readonly ICommandHandler<TCommand> _decoratedHandler;
    private readonly IPrincipal _currentPrincipal;
    private readonly ILogger _logger;

    public AuthorizationCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler, IPrincipal currentPrincipal, ILogger logger)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        _currentPrincipal = currentPrincipal ?? throw new ArgumentNullException(nameof(currentPrincipal));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(AuthorizationCommandHandlerDecorator<TCommand>));
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        Authorize(command);

        await _decoratedHandler.HandleAsync(command, cancellationToken);
    }

    private void Authorize(TCommand command)
    {
        // TODO: Read roles from query or queryHandler and check here...
        // Another option would be to store matrix of roles externally in config or database.
        string commandName = command.GetType().GetFriendlyName();
        if (typeof(TCommand).Namespace.Contains("Admin") && !_currentPrincipal.IsInRole("Admin"))
            throw new SecurityException();

        _logger.Information("User {Principal} has been authorized to execute {CommandName}",
            _currentPrincipal?.Identity?.Name, commandName);
    }
}