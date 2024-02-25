using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class UpdatePassword
    {
        public string? Email { get; set; }
        [Required]
        public string? Token { get; set; }
        [Required]
        [MaxLength(24)]
        public string? Password { get; set; }
        [Required]
        [Compare("Password")]
        [MaxLength(24)]
        public string? ConfirmPassword { get; set; }
    }
}
