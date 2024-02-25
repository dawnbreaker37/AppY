using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SignUp
    {
        [MaxLength(100, ErrorMessage = "Too long for email")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [MaxLength(24, ErrorMessage = "Max length for username is 40 chars")]
        [DisplayName("Username")]
        [Required(ErrorMessage = "Choose a username for you")]
        public string? Username { get; set; }
        [MaxLength(24, ErrorMessage = "Max length for password is 24 chars")]
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [MaxLength(24, ErrorMessage = "Max length for password is 24 chars")]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords are not same")]
        public string? ConfirmPassword { get; set; }
        [MaxLength(6)]
        public string? ReserveCode { get; set; }
    }
}
