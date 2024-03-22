using AppY.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class DiscussionMessageReply : Base
    {
        [MaxLength(2400)]
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public int ReactionId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("DiscussionMessage")]
        public int? MessageId { get; set; }
        public User? User { get; set; }
        public DiscussionMessage? DiscussionMessage { get; set; }
    }
}
