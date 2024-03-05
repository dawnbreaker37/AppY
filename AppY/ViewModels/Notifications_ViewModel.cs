using System.ComponentModel.DataAnnotations;

namespace AppY.ViewModels
{
    public class Notifications_ViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Notification title is required")]
        [MaxLength(45, ErrorMessage = "Title may contain max 45")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Notification description (main text) is required")]
        [MaxLength(600, ErrorMessage = "Description may contain max 600")]
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "User's ID is required")]
        public int UserId { get; set; }
        public bool IsPinned { get; set; }
        public bool IsUntouchable { get; set; }
        public int NotificationCategoryId { get; set; }
    }
}
