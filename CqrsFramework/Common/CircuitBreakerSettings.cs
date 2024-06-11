namespace CqrsFramework.Common;

public class CircuitBreakerSettings
{
    /// <summary>
    /// Enable/disable circuit breaker.
    /// </summary>
    public bool Enabled { get; set; } = false;
    
    /// <summary>
    /// The number of exceptions that can occur before the circuit breaker opens.
    /// </summary>
    public int ExceptionsAllowedBeforeBreaking { get; set; } = 5;

    /// <summary>
    /// The duration of time in seconds that the circuit breaker will remain open before resetting.
    /// </summary>
    public int DurationOfBreakInSeconds { get; set; } = 30;

    /// <summary>
    /// The exception predicate to filter the type of exception this policy can handle.
    /// </summary>
    public List<Func<Exception, bool>>? ExceptionPredicates { get; set; } = null;
}