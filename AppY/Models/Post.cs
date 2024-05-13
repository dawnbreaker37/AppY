using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class Post : Base
    {
        [MaxLength(2000)]
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? AvailableTill { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPinned { get; set; }
        public bool CanBeForwarded { get; set; }
        public int Style { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
