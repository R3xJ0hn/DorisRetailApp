using System;

namespace DorisApp.Data.Library.DTO
{
    public class RequestInventoryPageDTO: RequestPageDTO
    {
        public bool IncludeSold { get; set; }
        public string? Sku { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
