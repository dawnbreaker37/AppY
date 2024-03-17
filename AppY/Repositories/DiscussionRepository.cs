using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AppY.Repositories
{
    public class DiscussionRepository : Base<DiscussionRepository>, IDiscussion
    {
        private readonly Context _context;
        private readonly INotification _notification;
        public DiscussionRepository(Context context, INotification notification) : base(context)
        {
            _context = context;
            _notification = notification;
        }

        public async Task<int> CreateDiscussionAsync(Discussion_ViewModel Model)
        {
            if(!String.IsNullOrEmpty(Model.Name))
            {
                if(Model.IsPrivate) Model.Password = Guid.NewGuid().ToString("D").Substring(0, 6);

                Discussion discussion = new Discussion
                {
                    AvatarUrl = null,
                    Name = Model.Name,
                    CreatedAt = DateTime.Now,
                    Description = null,
                    CreatorId = Model.UserId,
                    RemovedAt = null,
                    IsDeleted = false,
                    IsPrivate = Model.IsPrivate,
                    Password = Model.Password,
                    Shortlink = "did_" + Guid.NewGuid().ToString("D").Substring(2, 10)
                };

                await _context.AddAsync(discussion);
                await _context.SaveChangesAsync();

                DiscussionUsers discussionUsers = new DiscussionUsers
                {
                    DiscussionId = discussion.Id,
                    UserId = Model.UserId,
                    JoinedAt = DateTime.Now,
                    IsDeleted = false
                };

                await _context.AddAsync(discussionUsers);
                await _context.SaveChangesAsync();

                return discussion.Id;
            }
            return 0;
        }

        public async Task<int> DeleteDiscussionAsync(int Id, int UserId, string? DiscussionName)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result = await _context.Discussions.AsNoTracking().Where(d => d.Id == Id && d.CreatorId == UserId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true).SetProperty(d => d.RemovedAt, DateTime.Now));
                if (Result > 0)
                {
                    IQueryable<int>? AllMembersIds_Preview = _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && d.UserId != UserId && !d.IsDeleted).Select(d => d.UserId != null ? (int)d.UserId : 0);
                    if(AllMembersIds_Preview is not null)
                    {
                        List<int>? AllMembersIds = await AllMembersIds_Preview.ToListAsync();
                        if(AllMembersIds.Count > 0)
                        {
                            Notifications_ViewModel Notification = new Notifications_ViewModel()
                            {
                                Title = DiscussionName + " discussion has been deleted",
                                Description = "Hi! The " + DiscussionName + " discussion has been recently deleted by his owner. We've sent this message to inform you that you won't be able no more to enter it. For additional information chat with that discussion owner. Thank You",
                                IsUntouchable = false,
                                IsPinned = false,
                                NotificationCategoryId = 5,
                                UserId = 0
                            };

                            int NotificationsResult = await _notification.SendGroupNotificationsAsync(Notification, AllMembersIds);
                            if (NotificationsResult > 0) return NotificationsResult;
                        }
                    }
                    return Id;
                }
            }
            return 0;
        }

        public async Task<int> RestoreDiscussionAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                DateTime? RemovedAt_NoTime = null;
                int Result = await _context.Discussions.AsNoTracking().Where(d => d.Id == Id && d.CreatorId == UserId && d.IsDeleted && d.RemovedAt.HasValue).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, false).SetProperty(d => d.RemovedAt, RemovedAt_NoTime));
                if (Result > 0) return Id;
            }
            return 0;
        }

        public async Task<bool> EditDiscussionAsync(Discussion_ViewModel Model)
        {
            if(Model.UserId != 0 && Model.Id != 0 && !String.IsNullOrEmpty(Model.Name) && !String.IsNullOrEmpty(Model.Shortlink))
            {
                bool IsUserInsistsInThisDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Model.Id && d.UserId == Model.UserId && !d.IsDeleted);
                if(IsUserInsistsInThisDiscussion)
                {
                    bool IsThisShortlinkUnique = await _context.Discussions.AsNoTracking().AnyAsync(d => d.Id != Model.Id && d.Shortlink!.ToLower() == Model.Shortlink.ToLower());
                    if (!IsThisShortlinkUnique)
                    {
                        int Result = await _context.Discussions.AsNoTracking().Where(d => d.Id == Model.Id && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.Name, Model.Name).SetProperty(d => d.Shortlink, Model.Shortlink).SetProperty(d => d.Description, Model.Description));
                        if (Result != 0) return true;
                    }
                }
            }
            return false;
        }

        public async Task<Discussion?> GetDiscussionInfoAsync(int Id)
        {
            if (Id != 0) return await _context.Discussions.AsNoTracking().Select(d => new Discussion { Id = d.Id, Name = d.Name, Description = d.Description, AvatarUrl = d.AvatarUrl, Shortlink = d.Shortlink, CreatedAt = d.CreatedAt, IsPrivate = d.IsPrivate, Password = !d.IsPrivate ? null : d.Password, CreatorId = d.CreatorId, IsDeleted = d.IsDeleted }).FirstOrDefaultAsync(d => d.Id == Id && !d.IsDeleted);
            else return null;
        }

        public async Task<int> GetMembersCountAsync(int Id)
        {
            if (Id != 0) return await _context.DiscussionUsers.AsNoTracking().CountAsync(d => d.DiscussionId == Id && !d.IsDeleted);
            else return 0;
        }

        public IQueryable<DiscussionUsers>? GetMembersInfo(int Id)
        {
            if (Id != 0) return _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).Select(d => new DiscussionUsers { JoinedAt = d.JoinedAt, UserId = d.UserId, UserName = d.User!.PseudoName }).OrderByDescending(d => d.JoinedAt);
            else return null;
        }

        public IQueryable<DiscussionShortInfo>? GetUserDiscussions(int Id)
        {
            if (Id != 0) return _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == Id && !d.IsDeleted).Select(d => new DiscussionShortInfo { Id = d.Id, JoinedAt = d.JoinedAt, IsMuted = d.IsMuted, IsPinned = d.IsPinned, DiscussionId = d.DiscussionId, DiscussionName = d.Discussion != null ? d.Discussion.Name : null, CreatedAt = d.Discussion != null ? d.Discussion.CreatedAt : DateTime.Now });
            else return null;
        }

        public IQueryable<DiscussionShortInfo>? GetUserDeletedDiscussions(int Id)
        {
            if (Id != 0) return _context.Discussions.AsNoTracking().Where(d => d.CreatorId == Id && d.IsDeleted).Select(d => new DiscussionShortInfo { CreatedAt = d.CreatedAt, DeletedAt = d.RemovedAt, DiscussionName = d.Name, DiscussionId = d.Id, UserId = Id });
            else return null;
        }

        public async Task<bool> IsShortLinkFreeAsync(int Id, string? Shortlink)
        {
            if (Id != 0 && !String.IsNullOrEmpty(Shortlink))
            {
                bool Result = await _context.Discussions.AsNoTracking().AnyAsync(d => d.Id != Id && d.Shortlink!.ToLower() == Shortlink.ToLower());
                return Result ? false : true;
            }
            else return false;
        }

        public async Task<bool> IsThisDiscussionMutedAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0) return await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Id && d.UserId == UserId && d.IsMuted);
            else return false;
        }

        public  async Task<int> MuteAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && d.UserId == UserId).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsMuted, true));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async Task<int> UnmuteAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && d.UserId == UserId).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsMuted, false));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async Task<int> GetDeletedDiscussionsCountAsync(int Id)
        {
            if (Id != 0) return await _context.Discussions.AsNoTracking().CountAsync(d => d.CreatorId == Id && d.IsDeleted);
            else return 0;
        }

        public async Task<int> AddMemberAsync(int Id, int AdderId, int UserId)
        {
            if (Id != 0 && AdderId != 0 && UserId != 0 && (UserId != AdderId))
            {
                bool IsThisAdderHasPermission = await _context.Discussions.AsNoTracking().AnyAsync(d => d.Id == Id && d.CreatorId == AdderId);
                if (IsThisAdderHasPermission)
                {
                    DiscussionUsers? AddingUserInfo = await _context.DiscussionUsers.AsNoTracking().FirstOrDefaultAsync(d => d.UserId == UserId && d.DiscussionId == Id);
                    if (AddingUserInfo is not null)
                    {
                        if (AddingUserInfo.IsDeleted)
                        {
                            AddingUserInfo.IsDeleted = false;
                            await _context.SaveChangesAsync();

                            return UserId;
                        }
                        else return -256;
                    }
                    else
                    {
                        DiscussionUsers discussionUsers = new DiscussionUsers
                        {
                            DiscussionId = Id,
                            UserId = UserId,
                            IsDeleted = false,
                            JoinedAt = DateTime.Now
                        };
                        await _context.AddAsync(discussionUsers);
                        await _context.SaveChangesAsync();

                        return UserId;
                    }
                }
            }
            else return -128;
            return 0;
        }
    }
}
