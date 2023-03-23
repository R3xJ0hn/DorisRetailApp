using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DorisApp.Data.Library.Model
{
    public class BrandModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Brand Name")]
        public string BrandName { get; set; }
        public string? StoredImageName { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
