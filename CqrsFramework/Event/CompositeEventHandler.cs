using System.Diagnostics;
using CqrsFramework.Auditing;
using CqrsFramework.Common;
using CqrsFramework.Logging;

namespace CqrsFramework.Event;

public interface ICompositeEventHandler<TEvent> : IEventHandler<TEvent> 
    where TEvent : IEvent
{
}

[DebuggerStepThrough]
public class CompositeEventHandler<TEvent> : ICompositeEventHandler<TEvent> 
    where TEvent: IEvent
{
    private readonly IEnumerable<IEventHandler<TEvent>> _eventHandlers;
    private readonly ILogger _logger;
    private readonly IEventAuditor _eventAuditor;

    public CompositeEventHandler(IEnumerable<IEventHandler<TEvent>> eventHandlers, ILogger logger, IEventAuditor eventAuditor)
    {
        _eventHandlers = eventHandlers;
        _eventAuditor = eventAuditor ?? throw new ArgumentNullException(nameof(eventAuditor));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        
        _logger = logger.ForContext(typeof(CompositeEventHandler<TEvent>));
    }

    /// <summary>
    /// Dispatches any events to registered event handlers and logs occurrences for each event handler.
    /// </summary>
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        var eventName = @event.GetType().GetFriendlyName();
        _logger.Debug("Dispatching {EventName} event to {SubscriberCount} subscribers",
            eventName, _eventHandlers.Count());

        await _eventAuditor.AuditAsync(@event, cancellationToken);
        
        if (_eventHandlers != null && _eventHandlers.Any())
        {
            // TODO: Parallelize this?
            foreach (var eventHandler in _eventHandlers)
            {
                await eventHandler.HandleAsync(@event, cancellationToken);
            }
            
            // List<Task> tasks = new List<Task>();
            // foreach (var eventHandler in _eventHandlers)
            // {
            //     tasks.Add(eventHandler.HandleAsync(@event, cancellationToken));
            // }
            // await Task.WhenAll(tasks);
        }
        else
        {
            _logger.Warning("No event handler found for {EventName}", eventName);
        }
    }
}