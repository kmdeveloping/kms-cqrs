namespace cqrsCore.Common;

public interface IRetryable
{
  /// <summary>
  /// Retry settings for resilience.
  /// </summary>
  RetrySettings RetrySettings { get; set; }
}