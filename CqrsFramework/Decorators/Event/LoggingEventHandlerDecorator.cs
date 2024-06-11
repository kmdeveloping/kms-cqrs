using System.Diagnostics;
using CqrsFramework.Common;
using CqrsFramework.Event;
using CqrsFramework.Logging;
using SimpleInjector;

namespace CqrsFramework.Decorators.Event;

[DebuggerStepThrough]
public class LoggingEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
    where TEvent: IEvent
{
    private readonly IEventHandler<TEvent> _decoratedHandler;
    private readonly ILogger _logger;
    private readonly string _handlerName;
        
    public LoggingEventHandlerDecorator(IEventHandler<TEvent> decoratedHandler, ILogger logger, DecoratorContext decoratorContext)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        if(logger == null) throw new ArgumentNullException(nameof(logger));
        _logger = logger.ForContext(typeof(LoggingEventHandlerDecorator<TEvent>));
        if (decoratorContext == null) throw new ArgumentNullException(nameof(decoratorContext));
        _handlerName = decoratorContext.ImplementationType.GetFriendlyName();
    }
        
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));

        var eventName = @event.GetType().GetFriendlyName();

        using (_logger.PushProperty("Event", @event, true))
        {
            _logger.Debug("Handling event {EventName} using handler {EventHandler}", 
                eventName, _handlerName);
                
            var sw = Stopwatch.StartNew();
            try
            {
                await _decoratedHandler.HandleAsync(@event, cancellationToken);
                sw.Stop();
                _logger.Debug("Handled event {EventName} in {EventExecutionTime} msec", 
                    eventName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.Error(ex, "Failed handling event {EventName} after {EventExecutionTime} msec",
                    eventName, sw.ElapsedMilliseconds);
                throw;
            }   
        }
    }
}