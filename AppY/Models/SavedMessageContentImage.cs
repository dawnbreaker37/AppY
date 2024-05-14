using System.ComponentModel.DataAnnotations.Schema;

namespace AppY.Models
{
    public class SavedMessageContentImage : Base
    {
        public string? Name { get; set; }
        [ForeignKey("SavedMessageContent")]
        public int SavedMessageId { get; set; }
        public SavedMessageContent? SavedMessageContent { get; set; }
        [NotMapped]
        public int SkipCount { get; set; }
        [NotMapped]
        public int FullCount { get; set; }
    }
}
