using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.Model
{
    public class AuthenticationUserModel
    {
        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }

    }
}
