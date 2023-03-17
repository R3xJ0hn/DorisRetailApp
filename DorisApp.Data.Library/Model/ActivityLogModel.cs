using System;

namespace DorisApp.Data.Library.Model
{
    public class ActivityLogModel
    {
        public string Message { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public DateTime Date { get; set; }

        public string DeviceName { get; set; }
    }
}
