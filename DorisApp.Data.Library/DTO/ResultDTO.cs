
using System;

namespace DorisApp.Data.Library.DTO
{
    public class ResultDTO<T>
    {
        public T Data { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public string? ReasonPhrase { get; set; }
        public int ErrorCode { get; set; }
        public DateTime DateRequstedUTC { get; set; } = DateTime.UtcNow;

    }
}
