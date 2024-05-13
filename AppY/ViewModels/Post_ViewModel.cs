using AppY.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class Post_ViewModel
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "Post must contain text")]
        [MaxLength(2000, ErrorMessage = "Post text must be less than 2000 chars")]
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? File { get; set; }
        public int AvailableTill { get; set; }
        public DateTime? AvailableTillDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPinned { get; set; }
        public bool CanBeForwarded { get; set; }
        public int Style { get; set; }
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
