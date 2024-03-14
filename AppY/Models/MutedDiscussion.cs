using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class MutedDiscussion
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Discussion")]
        public int DiscussionId { get; set; }
        public User? User { get; set; }
        public Discussion? Discussion { get; set; }

    }
}
