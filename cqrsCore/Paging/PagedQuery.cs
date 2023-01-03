using cqrsCore.Query;

namespace cqrsCore.Paging;

public class PagedQuery<TQuery, TResult> : IQuery<Paged<TResult>> where TQuery : IQuery<IQueryable<TResult>>
{
  public TQuery Query { get; set; }
  public PageInfo PageInfo { get; set; }
}