using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class ChangePassword
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? OldPassword { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Min length for new password is 8 characters")]
        [MaxLength(24, ErrorMessage = "Max length for new password is 24 characters")]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]   
        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Min length for new password is 8 characters")]
        [MaxLength(24, ErrorMessage = "Max length for new password is 24 characters")]
        [Compare("NewPassword", ErrorMessage = "New passwords are not same")]
        public string? ConfirmPassword { get; set; }
    }
}
