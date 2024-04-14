using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class DiscussionMessageImage : Base
    {
        public string? Url { get; set; }
        [ForeignKey("DiscussionMessage")]
        public int MessageId { get; set; }
        public DiscussionMessage? DiscussionMessage { get; set; }
        [NotMapped]
        public int SkipCount { get; set; }
    }
}
