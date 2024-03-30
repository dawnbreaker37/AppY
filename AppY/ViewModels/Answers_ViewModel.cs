using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class Answers_ViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(2400, ErrorMessage = "Your answer's too long. Please, shorter it a bit")]
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEdited { get; set; }
        public int? UserId { get; set; }
        [Required]
        public int MessageId { get; set; }
        [Required]
        public int DiscussionOrChatId { get; set; }
    }
}
