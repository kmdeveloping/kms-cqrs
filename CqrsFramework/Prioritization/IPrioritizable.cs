namespace CqrsFramework.Prioritization;

/// <summary>
/// When applied to a resource, indicates that resource is prioritizable using the specified
/// weight of the <see cref="Priority"/>.
/// </summary>
public interface IPrioritizable
{
    /// <summary> The weight/value of the resource's priority, relative to other resources. </summary>
    /// <remarks>A higher value indicates a higher priority, while a lower number indicates a lower priority.</remarks>
    int Priority { get; }
}