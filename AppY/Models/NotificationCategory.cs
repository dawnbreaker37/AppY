namespace AppY.Models
{
    public class NotificationCategory : Base
    {
        public string? Name { get; set; }
        public List<NotificationModel>? Notifications { get; set; }
    }
}
