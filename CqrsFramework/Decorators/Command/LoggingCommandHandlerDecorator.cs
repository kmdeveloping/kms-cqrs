using System.Diagnostics;
using CqrsFramework.Command;
using CqrsFramework.Common;
using CqrsFramework.Logging;
using SimpleInjector;

namespace CqrsFramework.Decorators.Command;

[DebuggerStepThrough]
public class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand: ICommand
{
    private readonly ICommandHandler<TCommand> _decoratedService;
    private readonly ILogger _logger;
    private readonly string _handlerName;
        
    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> decoratedService, ILogger logger, DecoratorContext decoratorContext)
    {
        _decoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
        if(logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(LoggingCommandHandlerDecorator<TCommand>));
        if (decoratorContext == null) throw new ArgumentNullException(nameof(decoratorContext));
        _handlerName = decoratorContext.ImplementationType.GetFriendlyName();
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var commandName = command.GetType().GetFriendlyName();

        using (_logger.PushProperty("Command", command, true))
        {
            if (command.ExecuteAsNoOp)
            {    _logger.Debug("Handling NO-OP command {CommandName} using handler {CommandHandler}", 
                commandName, _handlerName);
            }
            else
            {
                _logger.Debug("Handling command {CommandName} using handler {CommandHandler}",
                    commandName, _handlerName);
            }

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