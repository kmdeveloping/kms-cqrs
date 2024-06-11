namespace CqrsFramework.Query;

public interface IQueryProcessor
{
    /// <summary>
    /// Processes the specified query and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The result of the query.</returns>
    Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}