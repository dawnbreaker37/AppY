using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SendReply
    { 
        [Required(ErrorMessage = "Message text is required")]
        [MaxLength(2400, ErrorMessage = "Reply text max chars count are restricted on 2400")]
        public string? Text { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DiscussionId { get; set; }
        [Required]
        public int MessageId { get; set; }
        public int IsAutoDeletable { get; set; }
        public string? ReplyText { get; set; }
    }
}
