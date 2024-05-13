using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SendMessage
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        [MaxLength(125)]
        public string? ReplyText { get; set; }
        [MaxLength(3400, ErrorMessage = "No more than 3400 chars for a message are required")]
        public string? Text { get; set; }
        public DateTime? SentAt { get; set; }
        public int IsAutoDeletable { get; set; }
        [Required]
        public int UserId { get; set; }
        [DataType(DataType.ImageUrl)]
        public IFormFileCollection? Images { get; set; }
        public int DiscussionId { get; set; }
        public int ChatId { get; set; }
        public int CurrentChatUserId { get; set; }
    }
}
