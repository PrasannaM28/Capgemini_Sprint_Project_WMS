namespace WMS.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
        = new List<T>();

    public int TotalRecords { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
