namespace CqrsFramework.Event;

public interface IEvent
{
    /// <summary> Gets or sets the time in UTC when the event was created. </summary>
    DateTime? TimeUtc { get; set; }
    /// <summary> Gets or sets the source of the event. </summary>
    string? Source { get; set; }
    /// <summary> Gets or sets the metadata of the event. </summary>
    IDictionary<string, string>? Metadata { get; set; }
}