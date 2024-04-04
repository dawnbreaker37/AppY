using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class SendEdit
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Message text is required")]
        [MaxLength(3400, ErrorMessage = "Reply text max chars count are restricted on 2400")]
        public string? Text { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DiscussionId { get; set; }
    }
}
