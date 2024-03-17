using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SendMessage
    {
        public int Id { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Empty messages are not required")]
        [MaxLength(3400, ErrorMessage = "No more than 3400 chars for a message are required")]
        public string? Text { get; set; }
        public DateTime SentAt { get; set; }
        public int ReactionId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsChecked { get; set; }
        public bool IsEdited { get; set; }
        public bool IsAutoDeletable { get; set; }
        public bool IsPinned { get; set; }
        public int UserId { get; set; }
        public bool IsForCurrentUser { get; set; }
        [Required]
        public int DiscussionId { get; set; }
        public int ChatId { get; set; }

    }
}
