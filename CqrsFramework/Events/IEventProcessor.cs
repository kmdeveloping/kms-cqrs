namespace CqrsFramework.Events;

public interface IEventProcessor
{
  Task ProcessAsync(IEvent @event, CancellationToken cancellationToken);
}