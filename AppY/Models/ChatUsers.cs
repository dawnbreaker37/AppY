using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class ChatUsers : Base
    {
        public DateTime JoinedAt { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsPinned { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public Chat? Chat { get; set; }
        public User? User { get; set; }

    }
}
