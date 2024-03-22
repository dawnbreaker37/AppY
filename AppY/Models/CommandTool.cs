using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class CommandTool : Base
    {
        [MaxLength(40)]
        public string? CommandCall { get; set; }
        [MaxLength(240)]
        public string? Description { get; set; }
        public string? Code { get; set; }
        [ForeignKey("Discussion")]
        public int DiscussionId { get; set; }
        public Discussion? Discussion { get; set; }
    }
}
