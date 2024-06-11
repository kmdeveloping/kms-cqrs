using System.Diagnostics;

namespace CqrsFramework.Paging;

[DebuggerStepThrough]
public class Paged<T>
{
  /// <summary> Contains information about the requested page. </summary>
  public PageInfo Paging { get; set; }

  /// <summary> The list of items for the current page. </summary>
  public T[] Items { get; set; }
}