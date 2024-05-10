using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class ForwardMessage
    {
        [Required]
        [MaxLength(3400, ErrorMessage = "Forwarding message cannot be too large")]
        public string? ForwardingText { get; set; }
        [Required]
        public int MessageId { get; set; }
        [Required]
        public int ToChatId { get; set; }
        [Required]
        public int FromChatId { get; set; }
        [MaxLength(3400, ErrorMessage = "Caption can't contain more than 3400 chars")]
        public string? Caption { get; set; }
        [Required]
        public int UserId { get; set; }
        public int CurrentChatUserId { get; set; }
        public bool IsFromDiscussion { get; set; }
    }
}
