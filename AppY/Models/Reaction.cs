using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class Reaction : Base
    {
        [MaxLength(90)]
        public string? ReactionCode { get; set; }
        public List<DiscussionMessageReaction>? DiscussionMessageReactions { get; set; }
    }
}
