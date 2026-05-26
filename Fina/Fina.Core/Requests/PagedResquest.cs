namespace Fina.Core.Requests;

public abstract class PagedResquest : Request
{
    public int PageSize { get; set; } = Configuration.DefaultPageSize;
    public int PageNumber { get; set; } = Configuration.DefaultPageNumber;
}
