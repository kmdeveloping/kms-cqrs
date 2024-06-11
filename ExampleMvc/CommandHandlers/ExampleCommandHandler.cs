using CqrsFramework.Command;
using ExampleMvc.CommandContracts;

namespace ExampleMvc.CommandHandlers;

public class ExampleCommandHandler : ICommandHandler<ExampleCommand>
{
  private readonly ILogger<ExampleCommandHandler> _logger;

  public ExampleCommandHandler(ILogger<ExampleCommandHandler> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
  
  public Task HandleAsync(ExampleCommand command, CancellationToken cancellationToken = default)
  {
    _logger.LogInformation("{CommandExampleName} has requested execution at {CommandTimeStamp}", command.ExampleName, command.TimeStamp);
    
    _logger.LogInformation("Updated name to email address {NewEmailUserName}", AddEmailToName(command.ExampleName));
    return Task.CompletedTask;
  }

  private string AddEmailToName(string name)
  {
    return $"{name}@some-email.com";
  }
}