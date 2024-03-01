using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class UpdatePassword
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }
        [Required(ErrorMessage = "Enter your new password")]
        [MinLength(8, ErrorMessage = "Min length of new password must be 8 characters")]
        [MaxLength(24, ErrorMessage = "Password must contain [8-24] characters")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Confirm your password")]
        [Compare("Password", ErrorMessage = "Passowords must be same")]
        [MinLength(8)]
        [MaxLength(24)]
        public string? ConfirmPassword { get; set; }
    }
}
