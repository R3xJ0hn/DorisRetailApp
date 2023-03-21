using System;

namespace DorisApp.Data.Library.Model
{
    public class BrandModel
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string? ImageName { get; set; }
        public string? StoredImageName { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
