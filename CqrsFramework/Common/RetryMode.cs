namespace CqrsFramework.Common;

/// <summary>
/// The retry mode when errors occur.
/// </summary>
public enum RetryMode
{
  /// <summary>
  /// Retries immediately after an error.
  /// </summary>
  SimpleRetry,

  /// <summary>
  /// On error, retries after a delay.
  /// </summary>
  RetryWithDelay,
        
  /// <summary>
  /// On error, retries after an exponential backoff delay.
  /// </summary>
  RetryWithExponentialBackoff
}