using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SendReply
    { 
        public int Id { get; set; }
        [Required(ErrorMessage = "Message text is required")]
        [MaxLength(2400, ErrorMessage = "Reply text max chars count are restricted on 2400")]
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        [Required]
        public int UserId { get; set; }
        public int DiscussionId { get; set; }
        [Required]
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public int IsAutoDeletable { get; set; }
        [DataType(DataType.ImageUrl)]
        public IFormFileCollection? Images { get; set; }
        public string? ReplyText { get; set; }
        public int CurrentChatUserId { get; set; }
    }
}
