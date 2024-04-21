using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class Chat_ViewModel
    {
        public int Id { get; set; }
        [MaxLength(120, ErrorMessage = "This name is too long. Try shorter one, please")]
        public string? Name { get; set; }
        [MaxLength(600, ErrorMessage = "The description is too heavy. Try to make it a bit lighter, please (max 600 chars)")]
        public string? Description { get; set; }
        [MaxLength(24, ErrorMessage = "Too long shortname for a chat")]
        public string? Shortname { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CreatorId { get; set; }
        public int SecondUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
