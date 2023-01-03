namespace cqrsCore.Events;

public interface IEventProcessor
{
  Task ProcessAsync(IEvent @event, CancellationToken cancellationToken);
}