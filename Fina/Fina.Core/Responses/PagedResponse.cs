using System;

namespace Fina.Core.Responses;

public class PagedResponse<TData> : Response<TData>
{
    public int CurrentPage { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
}
