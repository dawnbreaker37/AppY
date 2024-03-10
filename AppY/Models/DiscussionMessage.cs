using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class DiscussionMessage : Base
    {
        [MaxLength(3400)]
        public string? Text { get; set; }
        public bool IsEdited { get; set; }
        public DateTime SentAt { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Discussion")]
        public int DiscussionId { get; set; }
        public User? User { get; set; }
        public Discussion? Discussion { get; set; }
    }
}
