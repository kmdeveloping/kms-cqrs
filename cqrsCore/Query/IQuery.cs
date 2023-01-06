namespace cqrsCore.Query;

/// <summary>
/// Represents a query.
/// </summary>
public interface IQuery<TResult>
{
  TResult Result { get; set; }
}