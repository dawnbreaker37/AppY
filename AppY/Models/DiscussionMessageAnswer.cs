using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class DiscussionMessageAnswer : Base
    {
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        [ForeignKey("DiscussionMessage")]
        public int? MessageId { get; set; }
        public bool IsEdited { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User? User { get; set; }
        public DiscussionMessage? DiscussionMessage { get; set; }
    }
}
