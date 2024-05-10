using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SavedMessageContent_ViewModel
    {
        [MaxLength(600)]
        public string? Addition { get; set; }
        public int MessageId { get; set; }
        public bool IsReplying { get; set; }
        [MaxLength(90)]
        public string? ReplyText { get; set; }
        [MaxLength(3000)]
        public string? Text { get; set; }
        [MaxLength(120, ErrorMessage = "The note's too large (max: 900 chars)")]
        public string? Note { get; set; }
        [MaxLength(12, ErrorMessage = "The badge may contain up to 12 chars")]
        public string? Badge { get; set; }
        public int ChatId { get; set; }
        public int DiscussionId { get; set; }
        public int UserId { get; set; }
    }
}
