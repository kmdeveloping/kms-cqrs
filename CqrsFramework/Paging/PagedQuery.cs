using System.Diagnostics;
using CqrsFramework.Query;

namespace CqrsFramework.Paging;

[DebuggerStepThrough]
public class PagedQuery<TQuery, TResult> : IQuery<Paged<TResult>> where TQuery : IQuery<IQueryable<TResult>>
{
  public TQuery Query { get; set; }
  public PageInfo PageInfo { get; set; }
  public Paged<TResult> Result { get; set; }
}