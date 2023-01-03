namespace cqrsCore.Common;

public interface ITimeout
{
  int TimeoutInSeconds { get; }
}