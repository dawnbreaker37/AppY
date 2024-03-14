using AppY.Abstractions;
using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class DiscussionMessageRepository : Message
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;

        public DiscussionMessageRepository(Context context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async override Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId)
        {
            if(Id != 0 && UserId != 0 && ChatOrDiscussionId != 0)
            {
                int Result = await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && d.UserId == UserId && d.DiscussionId == ChatOrDiscussionId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                if (Result != 0) return Id;
            }
            return 0;
        }

        public async override Task<int> EditMessageAsync(SendMessage Model)
        {
            if((!String.IsNullOrEmpty(Model.Text)) && Model.Id != 0 && Model.DiscussionId != 0 && Model.UserId != 0)
            {
                int DaysPassed = await _context.DiscussionMessages.Where(d => d.Id == Model.Id && d.DiscussionId == Model.DiscussionId && !d.IsDeleted && d.UserId == Model.UserId).Select(d => DateTime.Now.Subtract(d.SentAt).Days).FirstOrDefaultAsync();
                if (DaysPassed <= 4)
                {
                    int Result = await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Model.Id && d.DiscussionId == Model.DiscussionId && !d.IsDeleted && d.UserId == Model.UserId).ExecuteUpdateAsync(d => d.SetProperty(d => d.Text, Model.Text).SetProperty(d => d.IsEdited, true));
                    if (Result != 0) return Model.Id;
                }
            }
            return 0;
        }

        public async override Task<DiscussionMessage?> GetMessageInfoAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0) return await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && !d.IsDeleted && d.UserId == UserId).Select(d => new DiscussionMessage { Text = d.Text, IsChecked = d.IsChecked, IsEdited = d.IsEdited, SentAt = d.SentAt, UserId = d.UserId }).FirstOrDefaultAsync();
            else return null;
        }

        public override IQueryable<DiscussionMessage>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount)
        {
            if (Id != 0 && UserId != 0)
            {
                return _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).Select(d => new DiscussionMessage { Id = d.Id, IsEdited = d.IsEdited, SentAt = d.SentAt, Text = d.Text, IsChecked = d.IsChecked, UserId = d.UserId, UserPseudoname = d.UserId == UserId ? null : d.User!.PseudoName }).OrderBy(d => d.SentAt).Skip(SkipCount).Take(LoadCount);
            }
            else return null;
        }

        public async override Task<int> SendMessageAsync(SendMessage Model)
        {
            if (Model.DiscussionId != 0 && Model.UserId != 0 && !String.IsNullOrEmpty(Model.Text))
            {
                bool IsUserActiveInThisDiscussion = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Model.DiscussionId && d.UserId == Model.UserId && !d.IsDeleted);
                if (IsUserActiveInThisDiscussion)
                {
                    Model.SentAt = DateTime.Now;
                    DiscussionMessage discussionMessage = new DiscussionMessage
                    {
                        DiscussionId = Model.DiscussionId,
                        UserId = Model.UserId,
                        IsDeleted = false,
                        IsEdited = false,
                        SentAt = Model.SentAt,
                        Text = Model.Text,
                    };
                    await _context.AddAsync(discussionMessage);
                    await _context.SaveChangesAsync();

                    return discussionMessage.Id;
                }
            }
            return 0;
        }

        public override async Task<int> SentMessagesCountAsync(int DiscussionId)
        {
            if (DiscussionId != 0) return await _context.DiscussionMessages.AsNoTracking().CountAsync(d => d.DiscussionId == DiscussionId && !d.IsDeleted);
            else return 0;
        }
    }
}
