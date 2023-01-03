using cqrsCore.Paging;

namespace cqrsCore.Query;

public class PagedQueryHandler<TQuery, TResult> 
  : IQueryHandler<PagedQuery<TQuery, TResult>, Paged<TResult>> where TQuery : IQuery<IQueryable<TResult>>
{
  private readonly IQueryHandler<TQuery, IQueryable<TResult>> _handler;

  public PagedQueryHandler(IQueryHandler<TQuery, IQueryable<TResult>> handler)
  {
    _handler = handler ?? throw new ArgumentNullException(nameof(handler));
  }

  public async Task<Paged<TResult>> HandleAsync(PagedQuery<TQuery, TResult> query,
    CancellationToken cancellationToken)
  {
    var paging = query.PageInfo ?? new PageInfo();
    IQueryable<TResult> items = await _handler.HandleAsync(query.Query, cancellationToken);

    return new Paged<TResult>
    {
      Items = items.Skip(paging.PageIndex * paging.PageSize)
        .Take(paging.PageSize).ToArray(),
      Paging = paging
    };
  }
}