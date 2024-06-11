namespace CqrsFramework.Paging;

public class PageInfo
{
  /// <summary> Returns a PageInfo for a single page request. </summary>
  public static PageInfo SinglePage() =>
    new PageInfo { PageIndex = 0, PageSize = -1 };

  /// <summary> Gets the value indicating if the pageInfo is a single page request. </summary>
  public bool IsSinglePage() => PageIndex == 0 && PageSize == -1;
        
  /// <summary> The current page (zero-based). </summary>
  public int PageIndex { get; set; }

  /// <summary> The number of items per page. </summary>
  public int PageSize { get; set; } = 25;

  /// <summary> Total number of items available. </summary>
  public int TotalCount { get; set; }
}