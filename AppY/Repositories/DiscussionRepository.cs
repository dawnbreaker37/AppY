using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Data;
using System.Linq;

namespace AppY.Repositories
{
    public class DiscussionRepository : Base<DiscussionRepository>, IDiscussion
    {
        private readonly Context _context;
        private readonly INotification _notification;
        private readonly IUser _user;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DiscussionRepository(Context context, INotification notification, IUser user, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _context = context;
            _notification = notification;
            _user = user;
            _webHostEnvironment = webHostEnvironment;
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
                    AccessLevel = 2,
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
            if (Id != 0) return await _context.Discussions.AsNoTracking().Select(d => new Discussion { Id = d.Id, Name = d.Name, Description = d.Description, AvatarUrl = d.AvatarUrl, Shortlink = d.Shortlink, CreatedAt = d.CreatedAt, IsPrivate = d.IsPrivate, Password = !d.IsPrivate ? null : d.Password, CreatorId = d.CreatorId, IsDeleted = d.IsDeleted, Status = d.Status }).FirstOrDefaultAsync(d => d.Id == Id && !d.IsDeleted);
            else return null;
        }

        public async Task<Discussion?> GetDiscussionInfoAsync(string Id)
        {
            if (!String.IsNullOrWhiteSpace(Id)) return await _context.Discussions.AsNoTracking().Select(d => new Discussion { Id = d.Id, Name = d.Name, Description = d.Description, AvatarUrl = d.AvatarUrl, Shortlink = d.Shortlink, CreatedAt = d.CreatedAt, IsPrivate = d.IsPrivate, Password = !d.IsPrivate ? null : d.Password, CreatorId = d.CreatorId, IsDeleted = d.IsDeleted }).FirstOrDefaultAsync(d => d.Shortlink != null && d.Shortlink.ToLower() == Id.ToLower() && !d.IsDeleted);
            else return null;
        }

        public async Task<int> GetDiscussionIdByShortlinkAsync(string Shortlink)
        {
            if (!String.IsNullOrWhiteSpace(Shortlink)) return await _context.Discussions.AsNoTracking().Where(d => d.Shortlink != null && d.Shortlink.ToLower() == Shortlink.ToLower() && !d.IsDeleted).Select(d => d.Id).FirstOrDefaultAsync();
            else return 0;
        }

        public async Task<int> GetMembersCountAsync(int Id)
        {
            if (Id != 0) return await _context.DiscussionUsers.AsNoTracking().CountAsync(d => d.DiscussionId == Id && !d.IsDeleted);
            else return 0;
        }

        public IQueryable<DiscussionUsers>? GetMembersInfo(int Id)
        {
            if (Id != 0) return _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && (!d.IsDeleted || d.IsDeleted && d.IsBlocked)).Select(d => new DiscussionUsers { JoinedAt = d.JoinedAt, UserId = d.UserId, UserName = d.User!.PseudoName, AccessLevel = d.AccessLevel, IsBlocked = d.IsBlocked }).OrderByDescending(d => d.JoinedAt).OrderBy(d => d.IsBlocked);
            else return null;
        }

        public IQueryable<DiscussionShortInfo>? GetUserDiscussions(int Id)
        {
            if (Id != 0) return _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == Id && !d.IsDeleted).Select(d => new DiscussionShortInfo { Id = d.Id, JoinedAt = d.JoinedAt, IsMuted = d.IsMuted, IsPinned = d.IsPinned, DiscussionId = d.DiscussionId, DiscussionName = d.Discussion != null ? d.Discussion.Name : null, DiscussionAvatar = d.Discussion != null ? d.Discussion.AvatarUrl : null });
            else return null;
        }

        public IQueryable<DiscussionShortInfo>? GetUserDeletedDiscussions(int Id)
        {
            if (Id != 0) return _context.Discussions.AsNoTracking().Where(d => d.CreatorId == Id && d.IsDeleted).Select(d => new DiscussionShortInfo { CreatedAt = d.CreatedAt, DeletedAt = d.RemovedAt, DiscussionName = d.Name, DiscussionId = d.Id, UserId = Id });
            else return null;
        }

        public async Task<List<int?>?> GetUserDiscussionIds(int Id)
        {
            return await _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == Id && !d.IsDeleted && !d.IsBlocked).Select(d => d.DiscussionId).ToListAsync();
        }

        public async Task<List<DiscussionShortInfo>?> GetSimilarDiscussionsAsync(int Id, List<DiscussionShortInfo>? InitialList)
        {
            if (InitialList != null)
            {
                List<int?>? SecondUserIds = await _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == Id && !d.IsDeleted && !d.IsBlocked).Select(d => d.DiscussionId).ToListAsync();
                if (SecondUserIds != null)
                {
                    List<DiscussionShortInfo>? NewSimilarsList = new List<DiscussionShortInfo>();
                    foreach(DiscussionShortInfo Item in InitialList)
                    {
                        foreach (int? Item2 in SecondUserIds)
                        {
                            if (Item2.HasValue && Item2.Value == Item.DiscussionId) NewSimilarsList.Add(Item);
                        }
                    }

                    return NewSimilarsList;
                }
            }
            return null;
        }

        public async Task<int> GetDiscussionsCountAsync(int Id)
        {
            return await _context.DiscussionUsers.AsNoTracking().CountAsync(d => d.UserId == Id && !d.IsDeleted && !d.IsBlocked);
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
                            AccessLevel = 0,
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

        public async Task<int> JoinAsync(int Id, int UserId)
        {
            if(Id != 0 && UserId != 0)
            {
                DiscussionUsers? HasThisUserAnAccount = await _context.DiscussionUsers.FirstOrDefaultAsync(d => d.UserId == UserId && d.DiscussionId == Id && !d.IsBlocked);
                if(HasThisUserAnAccount is not null)
                {
                    HasThisUserAnAccount.IsDeleted = false;
                    await _context.SaveChangesAsync();

                    return Id;
                }
                else
                {
                    DiscussionUsers discussionUsers = new DiscussionUsers
                    {
                        DiscussionId = Id,
                        UserId = UserId,
                        IsDeleted = false,
                        AccessLevel = 0,
                        JoinedAt = DateTime.Now
                    };
                    await _context.AddAsync(discussionUsers);
                    await _context.SaveChangesAsync();

                    return Id;
                }
            }
            return 0;
        }

        public async Task<int> JoinToPrivateAsync(int Id, int UserId, string? Password)
        {
            if(Id != 0 && UserId != 0 && !String.IsNullOrWhiteSpace(Password))
            {
                DiscussionUsers? IsUserWasInThisDiscussion = await _context.DiscussionUsers.FirstOrDefaultAsync(d => d.DiscussionId == Id && d.UserId == UserId && !d.IsBlocked);
                if (IsUserWasInThisDiscussion is not null)
                {
                    if (IsUserWasInThisDiscussion.IsDeleted)
                    {
                        bool CheckPassword = await CheckDiscussionPasswordAsync(Id, Password);
                        if (CheckPassword)
                        {
                            IsUserWasInThisDiscussion.IsDeleted = false;
                            await _context.SaveChangesAsync();

                            return UserId;
                        }
                        else return -128;
                    }
                    else return -256;
                }
                else
                {
                    bool CheckPassword = await CheckDiscussionPasswordAsync(Id, Password);
                    if (CheckPassword)
                    {
                        DiscussionUsers discussionUsers = new DiscussionUsers
                        {
                            DiscussionId = Id,
                            UserId = UserId,
                            IsDeleted = false,
                            AccessLevel = 0,
                            IsBlocked = false,
                            IsMuted = false,
                            IsPinned = false,
                            JoinedAt = DateTime.Now
                        };
                        await _context.AddAsync(discussionUsers);
                        await _context.SaveChangesAsync();

                        return UserId;
                    }
                    else return -128;
                }
            }
            return 0;
        }

        public async Task<bool> CheckDiscussionPasswordAsync(int Id, string? Password)
        {
            if (Id != 0 && !String.IsNullOrWhiteSpace(Password))
            {
                bool Result = await _context.Discussions.AsNoTracking().AnyAsync(d => d.Id == Id && !d.IsDeleted && d.Password == Password);
                return Result;
            }
            else return false;
        }

        public async Task<int> LeaveAsync(int Id, int UserId)
        {
            if(Id != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && d.UserId == UserId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                if (Result > 0) return Id;
            }
            return 0;
        }

        public async Task<int> DeleteUserAsync(int Id, int DeleterId, int UserId)
        {
            if(Id != 0 && DeleterId != 0 && UserId != 0)
            {
                bool IsTheDeleterInTheDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.UserId == DeleterId && d.DiscussionId == Id && !d.IsDeleted);
                if(IsTheDeleterInTheDiscussion)
                {
                    bool IsTheUserInTheDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.UserId == UserId && d.DiscussionId == Id && !d.IsDeleted);
                    if(IsTheUserInTheDiscussion)
                    {
                        int DeleterAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, DeleterId);
                        int UserAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, UserId);
                        if(DeleterAccessLevel > UserAccessLevel)
                        {
                            int Result = await _context.DiscussionUsers.Where(d => d.UserId == UserId && d.DiscussionId == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                            if (Result > 0) return UserId;
                        }
                    }
                }
            }
            return 0;
        }

        public IQueryable<DiscussionShortInfo>? Find(string? Keyword)
        {
            if (!String.IsNullOrWhiteSpace(Keyword))
            {
                return _context.Discussions.AsNoTracking().Where(d => !d.IsDeleted && !d.IsPrivate && d.Name.ToLower().Contains(Keyword.ToLower())).Select(d => new DiscussionShortInfo { Id = d.Id, DiscussionName = d.Name, CreatedAt = d.CreatedAt }).OrderByDescending(d => d.CreatedAt).Take(125);
            }
            else return null;
        }

        public async Task<Discussion?> GetDiscussionShortInfoAsync(int Id, int UserId)
        {
            if (Id != 0)
            {
                return await _context.Discussions.AsNoTracking().Select(d => new Discussion { Id = d.Id, Name = d.Name, Description = d.Description, IsPrivate = d.IsPrivate, IsDeleted = d.IsDeleted, Shortlink = d.Shortlink }).FirstOrDefaultAsync(d => d.Id == Id && !d.IsDeleted);
            }
            else return null;
        }

        public async Task<bool> HasThisUserAccessToThisDiscussionAsync(int UserId, int DiscussionId)
        {
            if (UserId != 0 && DiscussionId != 0) return await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.UserId == UserId && d.DiscussionId == DiscussionId && !d.IsDeleted);
            else return false;
        }

        public async Task<int> PinAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == UserId && d.DiscussionId == Id && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsPinned, true));
                if (Result > 0) return Id;
            }
            return 0;
        }

        public async Task<int> UnpinAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == UserId && d.DiscussionId == Id && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsPinned, false));
                if (Result > 0) return Id;
            }
            return 0;
        }

        public async Task<int> ChangeAccessLevel(int Id, int ChangerId, int UserId, int AccessLevel)
        {
            //* each member can change the access level of a lower member
            //0 - standard user
            //1 - admin
            //2 - owner, so he/she may do everything (delete the discussion or other members also)
            if(Id != 0 && UserId != 0 && ChangerId != 0 && AccessLevel >= 0 && AccessLevel < 2)
            {
                int ChangerInformation = await _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && d.UserId == ChangerId && !d.IsDeleted).Select(d => d.AccessLevel).FirstOrDefaultAsync();
                DiscussionUsers? UserInformation = await _context.DiscussionUsers.FirstOrDefaultAsync(d => d.DiscussionId == Id && d.UserId == UserId && !d.IsDeleted);
                if((UserInformation != null) && (UserInformation.AccessLevel < ChangerInformation) && (AccessLevel <= ChangerInformation))
                {
                    UserInformation.AccessLevel = AccessLevel;
                    await _context.SaveChangesAsync();

                    return AccessLevel;
                }
            }
            return -1;
        }

        public async Task<int> BlockUserAsync(int Id, int BlockerId, int UserId)
        {
            if (Id != 0 && BlockerId != 0 && UserId != 0)
            {
                bool IsTheBlockerFromTheDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.UserId == BlockerId && d.DiscussionId == Id && !d.IsDeleted);
                if (IsTheBlockerFromTheDiscussion)
                {
                    int BlockerAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, BlockerId);
                    int UserAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, UserId);
                    if (BlockerAccessLevel > UserAccessLevel)
                    {
                        int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == UserId && d.DiscussionId == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsBlocked, true).SetProperty(d => d.IsDeleted, true));
                        if (Result > 0) return UserId;
                    }
                }
            }
            return 0;
        }

        public async Task<int> UnblockUserAsync(int Id, int UnblockerId, int UserId)
        {
            if(Id != 0 && UnblockerId != 0 && UserId != 0)
            {
                bool IsUnblockerInTheDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Id && d.UserId == UnblockerId && !d.IsDeleted);
                if(IsUnblockerInTheDiscussion)
                {
                    int UnblockerAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, UnblockerId);
                    if(UnblockerAccessLevel > 0)
                    {
                        int Result = await _context.DiscussionUsers.AsNoTracking().Where(d => d.DiscussionId == Id && d.UserId == UserId).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsBlocked, false));
                        if (Result > 0) return UserId;
                    }
                }
            }
            return 0;
        }

        public async Task<string?> SetDiscussionAvatarAsync(int Id, int UserId, IFormFile Image)
        {
            if(Id != 0 && UserId != 0 && Image != null)
            {
                bool CheckThisUsersAccess = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.Id == Id && d.UserId == UserId && !d.IsDeleted && d.AccessLevel > 0);
                if(CheckThisUsersAccess)
                {
                    string? FileExtension = Path.GetExtension(Image.FileName);
                    string? NewFileName = Guid.NewGuid().ToString("D").Substring(2, 12) + FileExtension;
                    using(FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "/DiscussionAvatars/" + NewFileName, FileMode.Create))
                    {
                        await Image.CopyToAsync(fs);
                        int Result = await _context.Discussions.AsNoTracking().Where(d => d.Id == Id && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.AvatarUrl, NewFileName));
                        if (Result > 0) return NewFileName;
                    }
                }
            }
            return null;
        }

        public async Task<bool> DeleteDiscussionAvatarAsync(int Id, int UserId)
        {
            if(Id != 0 && UserId != 0)
            {
                bool CheckAccessAbility = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Id && d.UserId == UserId && !d.IsDeleted && d.AccessLevel > 0);
                if(CheckAccessAbility)
                {
                    string? SomeNullValue = null;
                    string? AvatarUrl = await _context.Discussions.AsNoTracking().Where(d => d.Id == Id).Select(d => d.AvatarUrl).FirstOrDefaultAsync();
                    if(AvatarUrl != null)
                    {
                        int Result = await _context.Discussions.AsNoTracking().Where(d => d.Id == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.AvatarUrl, SomeNullValue));
                        if (Result > 0)
                        {
                            File.Delete(_webHostEnvironment.WebRootPath + "/DiscussionAvatars/" + AvatarUrl);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<string?> SetDiscussionStatusAsync(int Id, int UserId, string? Status)
        {
            if(Id > 0 && UserId > 0 && !String.IsNullOrWhiteSpace(Status) && Status.Length <= 180)
            {
                bool CheckUsersAccess = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.UserId == UserId && d.DiscussionId == Id && !d.IsDeleted && !d.IsBlocked && d.AccessLevel > 0);
                if(CheckUsersAccess)
                {
                    int Result = await _context.Discussions.AsNoTracking().Where(d => d.Id == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.Status, Status));
                    if (Result > 0) return Status;
                }
            }
            return null;
        }
    }
}
