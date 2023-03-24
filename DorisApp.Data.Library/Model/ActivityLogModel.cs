using System;

namespace DorisApp.Data.Library.Model
{
    public class ActivityLogModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int CreatedByUserId { get; set; }
        public string Username { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }
        public int StatusCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
