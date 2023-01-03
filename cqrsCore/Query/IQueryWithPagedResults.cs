using cqrsCore.Paging;

namespace cqrsCore.Query;

public interface IQueryWithPagedResults<TResult> : IQuery<Paged<TResult>>
{
  PageInfo Paging { get; set; }
}