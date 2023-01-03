using cqrsCore.Command;
using cqrsCore.Validation;

namespace cqrsCore.Decorators.Command;

public class ValidatingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
  where TCommand: ICommand
{
  private readonly IValidator _validator;
  private readonly ICommandHandler<TCommand> _decoratedHandler;

  public ValidatingCommandHandlerDecorator(IValidator validator, ICommandHandler<TCommand> decoratedHandler)
  {
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
  }

  public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
  {
    if (command == null) throw new ArgumentNullException(nameof(command));

    await _validator.ValidateAsync(command, cancellationToken);
    await _decoratedHandler.HandleAsync(command, cancellationToken);
  }
}