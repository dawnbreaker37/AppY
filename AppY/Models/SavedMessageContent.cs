using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppY.Models
{
    public class SavedMessageContent : Base
    {
        [MaxLength(600)]
        public string? Addition { get; set; }
        [MaxLength(3000)]
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        [MaxLength(28)]
        public string? Badge { get; set; }
        [ForeignKey("ChatMessage")]
        public int? ChatMessageId { get; set; }
        [ForeignKey("DiscussionMessage")]
        public int? DiscussionMessageId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime? PinnedAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsPinned { get; set; }
        public ChatMessage? ChatMessage { get; set; }
        public DiscussionMessage? DiscussionMessage { get; set; }
        public List<SavedMessageContentImage>? SavedMessageContentImages { get; set; }
        public User? User { get; set; }
        [NotMapped]
        public string? ImageUrl { get; set; }
        [NotMapped]
        public string? MessageText { get; set; }
        [NotMapped]
        public int ForwarderId { get; set; }
        [NotMapped]
        public string? ForwarderName { get; set; }
        [NotMapped]
        public string? MainImgUrl { get; set; }
        [NotMapped]
        public int SavedMessagesCount { get; set; }
    }
}
