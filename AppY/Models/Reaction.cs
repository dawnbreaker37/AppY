using System.ComponentModel.DataAnnotations;

namespace AppY.Models
{
    public class Reaction : Base
    {
        [MaxLength(90)]
        public string? ReactionCode { get; set; }
        public List<DiscussionMessageReaction>? DiscussionMessageReactions { get; set; }
    }
}
