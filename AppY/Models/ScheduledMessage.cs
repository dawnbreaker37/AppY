using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class ScheduledMessage : Base
    {
        public string? Text { get; set; }
        public DateTime ScheduledTime { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public bool IsAutoDeletable { get; set; }
        public int DiscussionId { get; set; }
        public int LiveDiscussionId { get; set; }
        public int ChatId { get; set; }
        public User? User { get; set; }
    }
}
