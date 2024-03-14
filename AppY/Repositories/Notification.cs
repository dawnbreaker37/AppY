using AppY.Data;
using AppY.Models;
using AppY.Interfaces;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class Notification : Base<Notification>, INotification
    {
        private readonly Context _context;
        public Notification(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<int> DeleteNotificationAsync(int Id, int UserId)
        {
            if(Id != 0 && UserId != 0)
            {
                int Result = await _context.Notifications.AsNoTracking().Where(u => u.Id == Id && u.UserId == UserId && !u.IsDeleted && !u.IsPinned && !u.IsUnkillable).ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDeleted, true));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async Task<bool> DeleteAncientNotificationsAsync(int UserId)
        {
            if (UserId != 0)
            {
                int Result = await _context.Notifications.AsNoTracking().Where(n => n.UserId == UserId && !n.IsDeleted && !n.IsPinned && n.SentAt.AddDays(5) > DateTime.Now).ExecuteUpdateAsync(n => n.SetProperty(n => n.IsDeleted, true));
                if (Result > 0) return true;
            }
            return false;
        } 

        public async Task<int> GetCheckedNotificationsCountAsync(int UserId)
        {
            if (UserId != 0) return await _context.Notifications.AsNoTracking().CountAsync(n => n.UserId == UserId && !n.IsDeleted && n.IsChecked);
            else return 0;
        }
        public async Task<int> GetMissedNotificationsCountAsync(int UserId)
        {
            if (UserId != 0) return await _context.Notifications.AsNoTracking().CountAsync(n => n.UserId == UserId && !n.IsDeleted && !n.IsChecked);
            else return 0;
        }

        public async Task<NotificationModel?> GetNotificationInfoAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                await MarkAsReadAsync(Id, UserId);
                return await _context.Notifications.AsNoTracking().Where(n => n.Id == Id && n.UserId == UserId && !n.IsDeleted).Select(n => new NotificationModel { Id = n.Id, Description = n.Description, IsPinned = n.IsPinned, SentAt = n.SentAt, NotificationCategoryId = n.NotificationCategoryId }).FirstOrDefaultAsync();
            }
            else return null;
        }

        public IQueryable<NotificationModel>? GetNotifications(int UserId, int SkipCount, int LoadCount)
        {
            if (UserId != 0) return _context.Notifications.AsNoTracking().Where(u => u.UserId == UserId && !u.IsDeleted).Select(n => new NotificationModel { Id = n.Id, IsPinned = n.IsPinned, Title = n.Title, IsChecked = n.IsChecked, SentAt = n.SentAt, IsUnkillable = n.IsUnkillable }).Skip(SkipCount).Take(LoadCount).OrderByDescending(p => p.IsPinned).ThenByDescending(u => u.SentAt);
            else return null;
        }

        public async Task<int> GetNotificationsCountAsync(int UserId)
        {
            if (UserId != 0) return await _context.Notifications.AsNoTracking().CountAsync(u => u.UserId == UserId && !u.IsDeleted);
            else return 0;
        }

        public async Task<int> MarkAsReadAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result  = await _context.Notifications.AsNoTracking().Where(u => u.Id == Id && u.UserId == UserId).ExecuteUpdateAsync(u => u.SetProperty(u => u.IsChecked, true));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async Task<int> PinAsync(int Id, int UserId)
        {
            if(Id != 0 && UserId != 0)
            {
                int Result = await _context.Notifications.AsNoTracking().Where(n => n.Id == Id && n.UserId == UserId && !n.IsPinned).ExecuteUpdateAsync(n => n.SetProperty(n => n.IsPinned, true));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async Task<int> UnpinAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result = await _context.Notifications.AsNoTracking().Where(n => n.Id == Id && n.UserId == UserId && n.IsPinned).ExecuteUpdateAsync(n => n.SetProperty(n => n.IsPinned, false));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async Task<int> SendNotificationAsync(Notifications_ViewModel Model)
        {
            if(Model.UserId != 0 && !String.IsNullOrEmpty(Model.Title) && !String.IsNullOrEmpty(Model.Description))
            {
                NotificationModel notification = new NotificationModel
                {
                    Title = Model.Title,
                    Description = Model.Description,
                    UserId = Model.UserId,
                    IsDeleted = false,
                    IsChecked = false,
                    IsPinned = false,
                    IsUnkillable = Model.IsUntouchable,
                    SentAt = DateTime.Now,
                    NotificationCategoryId = Model.NotificationCategoryId
                };
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();

                return notification.Id;
            }

            return 0;
        }

        public async Task<int> SendGroupNotificationsAsync(Notifications_ViewModel Model, List<int> Users)
        {
            if(Users.Count > 0 && !String.IsNullOrWhiteSpace(Model.Title) && !String.IsNullOrWhiteSpace(Model.Description))
            {
                List<Notifications_ViewModel>? NotificationsList = new List<Notifications_ViewModel>();
                foreach(int UserId in Users)
                {
                    Notifications_ViewModel Notification = new Notifications_ViewModel
                    {
                        Title = Model.Title,
                        Description = Model.Description,
                        IsDeleted = Model.IsDeleted,
                        IsPinned = Model.IsPinned,
                        IsUntouchable = Model.IsUntouchable,
                        NotificationCategoryId = Model.NotificationCategoryId,
                        UserId = UserId
                    };
                    NotificationsList.Append(Notification);
                }

                await _context.AddAsync(NotificationsList);
                await _context.SaveChangesAsync();

                return NotificationsList.Count;
            }

            return 0;
        }
    }
}
