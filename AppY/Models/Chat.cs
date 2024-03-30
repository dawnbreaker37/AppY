using System.ComponentModel.DataAnnotations;

namespace AppY.Models
{
    public class Chat : Base
    {
        [MaxLength(120)]
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatorId { get; set; }
        public List<User>? Users { get; set; }
        public List<ChatUsers>? ChatUsers { get; set; }
    }
}
