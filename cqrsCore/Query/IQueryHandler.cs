namespace cqrsCore.Query;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
  /// <summary>
  /// Handles the specified query, returning the results.
  /// Any applied decorators will be executed first (such as validation, etc.).
  /// </summary>
  /// <param name="query">The query to execute.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The results of the query as TResult.</returns>
  Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}