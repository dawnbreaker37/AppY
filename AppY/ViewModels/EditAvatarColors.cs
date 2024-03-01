using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class EditAvatarColors
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(6)]
        [MinLength(6)]
        public string? BgColor { get; set; }
        [MaxLength(6)]
        [MinLength(6)]
        public string? FgColor { get; set; }
    }
}
