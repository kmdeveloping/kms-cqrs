namespace CqrsFramework.Common;

public interface ITimeout
{
  int TimeoutInSeconds { get; }
}