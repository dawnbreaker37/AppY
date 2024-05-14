using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class ChatMessageImage
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        [ForeignKey("ChatMessage")]
        public int MessageId { get; set; }
        public ChatMessage? ChatMessage { get; set; }
    }
}
