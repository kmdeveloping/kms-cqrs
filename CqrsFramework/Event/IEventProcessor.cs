namespace CqrsFramework.Event;

public interface IEventProcessor
{
    Task ProcessAsync(IEvent @event, CancellationToken cancellationToken);
}