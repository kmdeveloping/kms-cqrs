using System.Diagnostics;
using cqrsCore.Command;
using cqrsCore.Common;
using cqrsCore.Logging;
using SimpleInjector;

namespace cqrsCore.Decorators.Command;

public class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand: ICommand
    {
        private readonly ICommandHandler<TCommand> _decoratedService;
        private readonly ILogger _logger;
        private readonly string _handlerName;
        
        public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> decoratedService, ILogger logger, DecoratorContext decoratorContext)
        {
            _decoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (decoratorContext == null) throw new ArgumentNullException(nameof(decoratorContext));
            _handlerName = decoratorContext.ImplementationType.GetFriendlyName();
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var commandName = command.GetType().GetFriendlyName();
            
            if (command.ExecuteAsNoOp)
            {    _logger.Debug("Handling NO-OP command {Command}: {@CommandJson} using handler {CommandHandler}", 
                    commandName, command, _handlerName);
            }
            else
            {
                _logger.Debug("Handling command {Command}: {@CommandJson} using handler {CommandHandler}",
                    commandName, command, _handlerName);
            }

            var sw = Stopwatch.StartNew();
            try
            {
                await _decoratedService.HandleAsync(command, cancellationToken);
                sw.Stop();
                _logger.Debug("Handled Command {Command} in {CommandExecutionTime} msec",
                    commandName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Error(ex, "Failed handling command {Command} after {CommandExecutionTime} msec", 
                    commandName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }