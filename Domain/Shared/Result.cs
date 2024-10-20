using System.Net;

namespace Domain.Shared
{
    public class Result<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
        public Error? Error { get; set; }
    }

    public class Result
    {
        public HttpStatusCode StatusCode { get; set; }
        public Error? Error { get; set; }
    }
}
