using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class SecretChatUsers
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("SecretChat")]
        public int SecretChatId { get; set; }
        public User? User { get; set; }
        public SecretChat? SecretChat { get; set; }
    }
}
