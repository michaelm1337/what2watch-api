using System.Net;

namespace Domain.Shared
{
    public class PagedResult<T>
    {
        public PagedResult(List<T>? items, int page, int pageSize, long totalSize)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalSize = totalSize;
        }

        public HttpStatusCode StatusCode { get; set; }
        public Error? Error { get; set; }

        public List<T>? Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public long TotalSize { get; }
        public bool HasNextPage => Page * PageSize < TotalSize;
        public bool HasPreviousPage => Page > 1;
    }
}
