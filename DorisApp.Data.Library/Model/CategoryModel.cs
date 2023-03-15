using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DorisApp.Data.Library.Model
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        public string? CategoryName { get; set; }

        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
