namespace cqrsCore.Paging;

public static class PagingExtensions
{
  /// <summary> Applies In-memory paging (using Linq to objects). </summary>
  /// <param name="collection">The collection for which to apply paging.</param>
  /// <param name="paging">The optional paging information. When null, default page values will be used.</param>
  /// <typeparam name="T">The type of objects to enumerate.</typeparam>
  /// <returns>The paged result using the specified paging parameters.</returns>
  public static Paged<T> Page<T>(this IEnumerable<T> collection, PageInfo paging)
  {
    paging = paging ?? new PageInfo();

    IEnumerable<T> items = paging.IsSinglePage()
      ? collection
      : collection.Skip(paging.PageIndex * paging.PageSize)
        .Take(paging.PageSize);
            
    if (paging.TotalCount == 0)
      paging.TotalCount = collection.Count();

    return new Paged<T> { Items = items.ToArray(), Paging = paging };
  }

  /// <summary> Applies paging using an efficient database query. </summary>
  /// <param name="collection">The collection for which to apply paging.</param>
  /// <param name="paging">The optional paging information. When null, default page values will be used.</param>
  /// <typeparam name="T">The type of objects to enumerate.</typeparam>
  /// <returns>The paged result using the specified paging parameters.</returns>
  public static Paged<T> Page<T>(this IQueryable<T> collection, PageInfo paging)
  {
    paging = paging ?? new PageInfo();

    IQueryable<T> items = paging.IsSinglePage()
      ? collection
      : collection.Skip(paging.PageIndex * paging.PageSize)
        .Take(paging.PageSize);
            
    if (paging.TotalCount == 0)
      paging.TotalCount = collection.Count();

    return new Paged<T> { Items = items.ToArray(), Paging = paging };
  }
}