using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Validation;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class ValidatingCommandHandlerDecorator<TCommand>(IValidator validator, ICommandHandler<TCommand> decoratedHandler) : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly IValidator _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    private readonly ICommandHandler<TCommand> _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        await _validator.ValidateAsync(command, cancellationToken);
        await _decoratedHandler.HandleAsync(command, cancellationToken);
    }
}