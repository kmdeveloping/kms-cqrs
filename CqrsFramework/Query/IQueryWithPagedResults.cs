using CqrsFramework.Paging;

namespace CqrsFramework.Query;

public interface IQueryWithPagedResults<TResult> : IQuery<Paged<TResult>>
{
  PageInfo Paging { get; set; }
}