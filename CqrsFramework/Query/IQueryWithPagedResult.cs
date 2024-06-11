using CqrsFramework.Paging;

namespace CqrsFramework.Query
{
    public interface IQueryWithPagedResult<TResult> : IQuery<Paged<TResult>>
    {
        PageInfo Paging { get; set; }
    }
}