using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class NotificationModel : Base
    {
        [MaxLength(45)]
        public string? Title { get; set; }
        [MaxLength(600)]
        public string? Description { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsChecked { get; set; }
        public bool IsPinned { get; set; }
        public bool IsUnkillable { get; set; }
        [ForeignKey("NotificationCategory")]
        public int NotificationCategoryId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public NotificationCategory? NotificationCategory { get; set; }
        [NotMapped]
        public string? NotificationIcon { get; set; }
    }
}
