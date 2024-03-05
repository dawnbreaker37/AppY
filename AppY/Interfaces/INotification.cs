using AppY.Models;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface INotification
    {
        public IQueryable<NotificationModel>? GetNotifications(int UserId, int SkipCount, int LoadCount);
        public Task<NotificationModel?> GetNotificationInfoAsync(int Id, int UserId);
        public Task<int> SendNotificationAsync(Notifications_ViewModel Model);
        public Task<int> DeleteNotificationAsync(int Id, int UserId);
        public Task<bool> DeleteAncientNotificationsAsync(int UserId);
        public Task<int> MarkAsReadAsync(int Id, int UserId);
        public Task<int> PinAsync(int Id, int UserId);
        public Task<int> UnpinAsync(int Id, int UserId);
        public Task<int> GetNotificationsCountAsync(int UserId);
        public Task<int> GetCheckedNotificationsCountAsync(int UserId);
        public Task<int> GetMissedNotificationsCountAsync(int UserId);
    }
}
