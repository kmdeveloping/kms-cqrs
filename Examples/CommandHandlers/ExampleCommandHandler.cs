using cqrsCore;
using cqrsCore.Command;
using Examples.CommandContracts;
using Microsoft.Extensions.Logging;

namespace Examples.CommandHandlers;

public class ExampleCommandHandler : ICommandHandler<ExampleCommand>
{
  private readonly ILogger<ExampleCommandHandler> _logger;

  public ExampleCommandHandler(ILogger<ExampleCommandHandler> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
  
  public async Task HandleAsync(ExampleCommand command, CancellationToken cancellationToken = default)
  {
    _logger.LogInformation("{CommandExampleName} has requested execution at {CommandTimeStamp}", command.ExampleName, command.TimeStamp);
  }
}