using System.ComponentModel.DataAnnotations;

namespace AccountManagmentAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
