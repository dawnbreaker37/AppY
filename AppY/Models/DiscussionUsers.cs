using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class DiscussionUsers
    {
        public int Id { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        [ForeignKey("Discussion")]
        public int? DiscussionId {get;set;}
        public User? User { get; set; }
        public Discussion? Discussion { get; set;}
        [NotMapped]
        public string? UserName { get; set; }
        [NotMapped]
        public string? UserShortlink { get; set; }
    }
}
