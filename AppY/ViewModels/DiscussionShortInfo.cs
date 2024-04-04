using AppY.Models;

namespace AppY.ViewModels
{
    public class DiscussionShortInfo
    {
        public int Id { get; set; }
        public string? DiscussionName { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? UserId { get; set; }
        public int? DiscussionId { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
        public string? DiscussionAvatar { get; set; }
        public string? LastMessageText { get; set; }
        public bool LastMessageIsChecked { get; set; }
        public DateTime LastMessageSentAt { get; set; }
    }
}
