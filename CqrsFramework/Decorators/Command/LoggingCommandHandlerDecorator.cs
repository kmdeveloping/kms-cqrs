using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using SimpleInjector;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class LoggingCommandHandlerDecorator<TCommand>(ICommandHandler<TCommand> decoratedService, ILogger logger, DecoratorContext decoratorContext) : ICommandHandler<TCommand> where TCommand: ICommand
{
    private readonly ICommandHandler<TCommand> _decoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
    private readonly ILogger _logger = logger.ForContext(typeof(LoggingCommandHandlerDecorator<TCommand>)) ?? throw new ArgumentNullException(nameof(logger));
    private readonly string _handlerName = decoratorContext.ImplementationType.GetFriendlyName() ?? throw new ArgumentNullException(nameof(decoratorContext));

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var commandName = command.GetType().GetFriendlyName();

        using (_logger.PushProperty("Command", command, true))
        {
            _logger.Debug(command.ExecuteAsNoOp ? "Handling NO-OP command {CommandName} using handler {CommandHandler}" : "Handling command {CommandName} using handler {CommandHandler}",
                commandName, _handlerName);

            var sw = Stopwatch.StartNew();
            try
            {
                await _decoratedService.HandleAsync(command, cancellationToken);
                sw.Stop();
                _logger.Debug("Handled Command {CommandName} in {CommandExecutionTime} msec",
                    commandName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Error(ex, "Failed handling command {CommandName} after {CommandExecutionTime} msec", 
                    commandName, sw.ElapsedMilliseconds);
                throw;
            }   
        }
    }
}