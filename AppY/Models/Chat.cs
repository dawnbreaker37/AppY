using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class Chat : Base
    {
        [MaxLength(120)]
        public string? Name { get; set; }
        [MaxLength(600)]
        public string? Description { get; set; }
        [MaxLength(24)]
        public string? Shortname { get; set; }
        public bool UnablePreview { get; set; }
        public bool ForbidMessageForwarding { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatorId { get; set; }
        public List<User>? Users { get; set; }
        public List<ChatUsers>? ChatUsers { get; set; }
        public List<ChatMessage>? ChatMessages { get; set; }
        [NotMapped]
        public string? DisabledTimeLeftText { get; set; }
    }
}
