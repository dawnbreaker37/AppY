using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class DiscussionMessageRepository : Base<DiscussionMessage>, IDiscussionMessage
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public DiscussionMessageRepository(Context context, UserManager<User> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> SendMessageAsync(SendMessage Model)
        {
            if(Model.DiscussionId != 0 && Model.UserId != 0 && !String.IsNullOrEmpty(Model.Text))
            {
                bool IsUserActiveInThisDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Model.DiscussionId && d.UserId == Model.UserId && !d.IsDeleted);
                if(IsUserActiveInThisDiscussion)
                {
                    DiscussionMessage discussionMessage = new DiscussionMessage
                    {
                        DiscussionId = Model.DiscussionId,
                        UserId = Model.UserId,
                        IsDeleted = false,
                        IsEdited = false,
                        SentAt = DateTime.Now,
                        Text = Model.Text,
                    };
                    await _context.AddAsync(discussionMessage);
                    await _context.SaveChangesAsync();

                    return discussionMessage.Id;
                }
            }
            return 0;
        }

        public async Task<int> SentMessagesCountAsync(int DiscussionId)
        {
            if (DiscussionId != 0) return await _context.DiscussionMessages.AsNoTracking().CountAsync(d => d.DiscussionId == DiscussionId && !d.IsDeleted);
            else return 0;
        }
    }
}
