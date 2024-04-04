using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class DiscussionMessageReaction : Base
    {
        [ForeignKey("Reaction")]
        public int ReactionId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("DiscussionMessage")]
        public int? DiscussionMessageId { get; set; }
        public User? User { get; set; }
        public Reaction? Reaction { get; set; }
        public DiscussionMessage? DiscussionMessage { get; set; }
        [NotMapped]
        public string? ReactionCode { get; set; }
    }
}
