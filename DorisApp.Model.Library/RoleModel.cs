using System;

namespace DorisApp.Model.Library
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
