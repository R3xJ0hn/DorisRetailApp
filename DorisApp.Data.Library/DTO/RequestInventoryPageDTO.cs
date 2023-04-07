using System;

namespace DorisApp.Data.Library.DTO
{
    public class RequestInventoryPageDTO: RequestPageDTO
    {
        public bool IncludeSold { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


}
