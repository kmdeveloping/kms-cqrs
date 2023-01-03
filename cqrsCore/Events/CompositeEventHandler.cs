using cqrsCore.Common;
using cqrsCore.Logging;

namespace cqrsCore.Events;

public class CompositeEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
{
  private readonly IEnumerable<IEventHandler<TEvent>> _eventHandlers;
  private readonly ILogger _logger;
  
  public CompositeEventHandler(IEnumerable<IEventHandler<TEvent>> eventHandlers, ILogger logger)
  {
    _eventHandlers = eventHandlers;
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
  
  /// <summary>
  /// Dispatches any events to registered event handlers and logs occurrences for each event handler.
  /// </summary>
  public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
  {
    var eventName = @event.GetType().GetFriendlyName();
    _logger.Debug("Dispatching {Event} event: {@EventJson} to {SubscriberCount} subscribers",
      eventName, @event, _eventHandlers.Count());

    if (_eventHandlers != null && _eventHandlers.Any())
    {
      foreach (var eventHandler in _eventHandlers)
      {
        await eventHandler.HandleAsync(@event, cancellationToken);
      }
    }
    else
    {
      _logger.Debug("No event handler found for {Event}", eventName);
    }
  }
}