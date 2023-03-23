using System;
using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.DTO
{
    public class RequestPageDTO : IEquatable<RequestPageDTO>
    {
        [Required]
        public int PageNo { get; set; }

        [Required]
        public int ItemPerPage { get; set; }

        [Required]
        public int OrderBy { get; set; }

        public string? LookFor { get; set; }

        public bool Equals(RequestPageDTO other)
        {
            if (other == null)
                return false;

            return PageNo == other.PageNo
                && ItemPerPage == other.ItemPerPage
                && OrderBy == other.OrderBy;
        }
    }

}
