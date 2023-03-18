using DorisApp.Data.Library.Enums;
using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.DTO
{
    public class RequestPageDTO
    {
        [Required]
        public int PageNo { get; set; }

        [Required]
        public int ItemPerPage { get; set; } 

        [Required]
        public int OrderBy { get; set; }

        public string? LookFor { get; set; }

    }

}
