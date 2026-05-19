using System;

namespace Fina.Core.Requests;

public abstract class PagedResquest
{
    public int PageSize { get; set; } = Configuration.DefaultPageSize;
    public int PageNumber { get; set; } = Configuration.DefaultPageNumber;
}
