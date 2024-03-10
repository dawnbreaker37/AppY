using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class DiscussionRepository : Base<DiscussionRepository>, IDiscussion
    {
        private readonly Context _context;
        public DiscussionRepository(Context context) : base(context)
        {
            _context = context;
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

        public IQueryable<DiscussionShortInfo>? GetUserDiscussion(int Id)
        {
            if (Id != 0) return _context.DiscussionUsers.AsNoTracking().Where(d => d.UserId == Id && !d.IsDeleted).Select(d => new DiscussionShortInfo { Id = d.Id, JoinedAt = d.JoinedAt, DiscussionId = d.DiscussionId, DiscussionName = d.Discussion != null ? d.Discussion.Name : null, CreatedAt = d.Discussion != null ? d.Discussion.CreatedAt : DateTime.Now });
            else return null;
        }

        public async Task<bool> IsShortLinkFree(int Id, string? Shortlink)
        {
            if (Id != 0 && !String.IsNullOrEmpty(Shortlink))
            {
                bool Result = await _context.Discussions.AsNoTracking().AnyAsync(d => d.Id != Id && d.Shortlink!.ToLower() == Shortlink.ToLower());
                return Result ? false : true;
            }
            else return false;
        }
    }
}
