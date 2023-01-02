using cqrsCore.Command;
using cqrsCore.Examples.ExampleCommand;

namespace cqrsCore.Examples.ExampleCommandHandler;

public class SayHelloCommandHandler : ICommandHandler<SayHelloCommand>
{
  private readonly ICqrsManager _cqrsManager;

  public SayHelloCommandHandler(ICqrsManager cqrsManager)
  {
    _cqrsManager = cqrsManager ?? throw new ArgumentNullException(nameof(cqrsManager));
  }
  
  public async Task HandleAsync(SayHelloCommand command, CancellationToken cancellationToken = default)
  {
    if (command is null) throw new ArgumentNullException(nameof(command));

    await _cqrsManager.ExecuteAsync(command, cancellationToken);
  }
}