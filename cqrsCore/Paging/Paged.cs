namespace cqrsCore.Paging;

public class Paged<T>
{
  /// <summary> Contains information about the requested page. </summary>
  public PageInfo Paging { get; set; }

  /// <summary> The list of items for the current page. </summary>
  public T[] Items { get; set; }
}