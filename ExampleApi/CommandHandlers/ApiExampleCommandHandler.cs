using cqrsCore.Command;
using cqrsCore.Logging;
using ExampleApi.Commands;

namespace ExampleApi.CommandHandlers;

public class ApiExampleCommandHandler : ICommandHandler<ApiExampleCommand>
{
  private readonly cqrsCore.Logging.ILogger<ApiExampleCommandHandler> _logger;

  public ApiExampleCommandHandler(cqrsCore.Logging.ILogger<ApiExampleCommandHandler> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
  
  public Task HandleAsync(ApiExampleCommand command, CancellationToken cancellationToken = default)
  {
    _logger.Information("{CommandRequester} executing {Command} at {CommandTimeStamp}", 
        command.inputName, nameof(command), DateTime.Now);

    command.Result = $"{command.inputName} executing noOp {command.ExecuteAsNoOp.ToString()}";
    
    return Task.FromResult(command.Result);
  }
}