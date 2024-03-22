using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class Discussion : Base
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(20)]
        public string? Shortlink { get; set; }
        public string? AvatarUrl { get; set; }
        [MaxLength(600)]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public bool IsPrivate { get; set; }
        public int CreatorId { get; set; }
        public int LastMessageId { get; set; }
        public string? Password { get; set; }
        public List<User>? Users { get; set; }
        public List<DiscussionMessage>? DiscussionMessages { get; set; }
        public List<CommandTool>? Commands { get; set; }
        [NotMapped]
        public string? CreatorName { get; set; }
        [NotMapped]
        public bool IsAlreadyInDiscussion { get; set; }
    }
}
