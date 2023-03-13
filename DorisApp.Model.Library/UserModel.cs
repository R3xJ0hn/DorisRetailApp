using System;
using System.Collections.Generic;
using System.Text;

namespace DorisApp.Model.Library
{
    public class UserModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string LastPasswordHash { get; set; } = string.Empty;
        public DateTime LastPasswordCanged { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
