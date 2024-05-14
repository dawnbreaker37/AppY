using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class ChatMessage : Base
    {
        [Required]
        [MaxLength(3400)]
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? PinnedAt { get; set; }
        public int IsAutodeletable { get; set; }
        public bool IsPinned { get; set; }
        public bool IsEdited { get; set; }
        public bool IsChecked { get; set; }
        public int RepliedMessageId { get; set; }
        [MaxLength(50)]
        public string? RepliesMessageText { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        [ForeignKey("ChatUsers")]
        public int? ChatUserId { get; set; }
        public User? User { get; set; }
        public Chat? Chat { get; set; }
        public ChatUsers? ChatUsers { get; set; }
        public List<ChatMessageImage>? ChatMessageImages { get; set; }
        [NotMapped]
        public string? SenderPseudoname { get; set; }
        [NotMapped]
        public string? MainImgUrl { get; set; }
    }
}
