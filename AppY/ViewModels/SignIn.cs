using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SignIn
    {
        [MaxLength(75)]
        [Required(ErrorMessage = "Email or Username is required")]
        public string? Email { get; set; }
        public bool IsViaUsername { get; set; }
        [MaxLength(24)]
        [Required(ErrorMessage = "Enter your password please")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }
}
