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
    }
}
