using System.Diagnostics;
using cqrsCore.Common;
using cqrsCore.Events;
using cqrsCore.Logging;
using SimpleInjector;

namespace cqrsCore.Decorators.Event;

public class LoggingEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
  where TEvent: IEvent
{
  private readonly IEventHandler<TEvent> _decoratedHandler;
  private readonly ILogger _logger;
  private readonly string _handlerName;
        
  public LoggingEventHandlerDecorator(IEventHandler<TEvent> decoratedHandler, ILogger logger, DecoratorContext decoratorContext)
  {
    _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    if (decoratorContext == null) throw new ArgumentNullException(nameof(decoratorContext));
    _handlerName = decoratorContext.ImplementationType.GetFriendlyName();
  }
        
  public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
  {
    if (@event == null) throw new ArgumentNullException(nameof(@event));

    var eventName = @event.GetType().GetFriendlyName();
            
    _logger.Debug("Handling event {EventName}: {@EventJson} using handler {EventHandler}", 
      eventName, @event, _handlerName);
                
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
      _logger.Error(ex, "Failed handling event after {EventExecutionTime} msec",
        sw.ElapsedMilliseconds);
      throw;
    }
  }
}