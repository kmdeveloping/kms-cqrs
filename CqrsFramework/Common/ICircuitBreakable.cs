namespace CqrsFramework.Common;

public interface ICircuitBreakable
{
    /// <summary>
    /// Circuit breaker settings for resilience.
    /// </summary>
    CircuitBreakerSettings CircuitBreakerSettings { get; set; }
}