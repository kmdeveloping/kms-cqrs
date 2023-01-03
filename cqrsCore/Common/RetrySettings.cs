namespace cqrsCore.Common;

public class RetrySettings
{
  /// <summary>
  /// Enable/disable retry on failure.
  /// </summary>
  public bool Enabled { get; set; } = false;

  /// <summary>
  /// The number of times to retry on failure.
  /// </summary>
  public int Count { get; set; } = 1;

  /// <summary>
  /// For exponential backoff. This will be used at the exponent portion (Delay^BackoffPower) for the delay between retry attempts.
  /// </summary>
  public int BackoffPower { get; set; } = 1;

  /// <summary>
  /// Delay (in seconds) between subsequent retry attempts.
  /// </summary>
  public int Delay { get; set; } = 0;

  /// <summary>
  /// When true, adds a random jitter between retry attempts (0 to 1000 milliseconds).
  /// </summary>
  public bool AddJitter { get; set; } = false;
}