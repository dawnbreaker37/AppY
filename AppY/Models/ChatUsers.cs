using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class ChatUsers : Base
    {
        [DataType(DataType.Password)]
        [MaxLength(16)]        
        public string? Password { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsPinned { get; set; }
        public bool IsMuted { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public Chat? Chat { get; set; }
        public User? User { get; set; }
        public List<ChatMessage>? ChatMessages { get; set; }
        [NotMapped]
        public bool PasswordAvailability { get; set; }
        [NotMapped]
        public string? ChatName { get; set; }
        [NotMapped]
        public string? ChatSecondUserName { get; set; }
        [NotMapped]
        public ChatMessage? LastMessageInfo { get; set; }

    }
}
