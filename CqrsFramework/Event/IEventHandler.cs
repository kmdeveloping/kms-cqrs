namespace CqrsFramework.Event;

public interface IEventHandler<in TEvent>
    where TEvent: IEvent
{
    public Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}