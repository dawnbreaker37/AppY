using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AppY.Repositories
{
    public class DiscussionMessageRepository : Message
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IMemoryCache _memoryCache;

        public DiscussionMessageRepository(Context context, UserManager<User> userManager, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        public async override Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId)
        {
            if(Id != 0 && UserId != 0 && ChatOrDiscussionId != 0)
            {
                int Result = await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && d.UserId == UserId && d.DiscussionId == ChatOrDiscussionId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                if (Result > 0)
                {
                    string? NullText = null;
                    await _context.DiscussionMessages.AsNoTracking().Where(d => d.RepliedMessageId == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.RepliesMsgShortText, NullText));
                    return Id;
                }
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
                    if (Result > 0)
                    {
                        await _context.DiscussionMessages.AsNoTracking().Where(d => d.RepliedMessageId == Model.Id && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.RepliesMsgShortText, Model.Text.Length > 40 ? Model.Text.Substring(0, 37) + "..." : Model.Text));
                        return Model.Id;
                    }
                }
            }
            return 0;
        }

        public async override Task<DiscussionMessage?> GetMessageInfoAsync(int Id, int UserId)
        {
            if (Id != 0 && UserId != 0)
            {
                return await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && !d.IsDeleted && d.UserId == UserId).Select(d => new DiscussionMessage { Text = d.Text, IsChecked = d.IsChecked, IsEdited = d.IsEdited, SentAt = d.SentAt, UserId = d.UserId }).FirstOrDefaultAsync();
            }
            else return null;
        }

        public override IQueryable<IGrouping<DateTime, DiscussionMessage>>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount)
        {
            if (Id != 0 && UserId != 0)
            {
                return _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).Select(d => new DiscussionMessage { Id = d.Id, Text = d.Text, DiscussionId = d.Id, IsAutoDeletable = d.IsAutoDeletable, IsChecked = d.IsChecked, IsEdited = d.IsEdited, SentAt = d.SentAt, UserId = d.UserId, IsPinned = d.IsPinned, RepliedMessageId = d.RepliedMessageId, RepliesMsgShortText = d.RepliesMsgShortText != null ? d.RepliesMsgShortText : "Deleted message" }).OrderBy(d => d.SentAt).Skip(SkipCount).Take(LoadCount).GroupBy(d => d.SentAt.Date);
                //return _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).Select(d => new DiscussionMessage { Id = d.Id, Text = d.Text, DiscussionId = d.Id, IsAutoDeletable = d.IsAutoDeletable, IsChecked = d.IsChecked, IsEdited = d.IsEdited, SentAt = d.SentAt, UserId = d.UserId, IsPinned = d.IsPinned, RepliedMessageId = d.RepliedMessageId, RepliesMsgShortText = _context.DiscussionMessages.AsNoTracking().Where(dm => dm.Id == d.RepliedMessageId && !dm.IsDeleted).Select(dm => dm.Text).FirstOrDefault() }).OrderBy(d => d.SentAt).Skip(SkipCount).Take(LoadCount).GroupBy(d => d.SentAt.Date);
            }
            else return null;
        }

        public async override Task<int> MarkAsReadAllMessagesAsync(int DiscussionId, int UserId)
        {
            if (DiscussionId != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == DiscussionId && d.UserId != UserId && !d.IsChecked).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsChecked, true));
                await _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == DiscussionId && d.UserId != UserId && d.IsAutoDeletable > 0 && DateTime.Now >= d.SentAt.AddMinutes(d.IsAutoDeletable)).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                if(Result > 0) return DiscussionId;
            }
            return 0;
        }

        public async override Task<int> MarkAsReadAsync(int MessageId, int UserId)
        {
            if(MessageId != 0 && UserId != 0)
            {
                int Result = await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == MessageId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsChecked, true));
                await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == MessageId && d.IsAutoDeletable > 0 && DateTime.Now >= d.SentAt.AddMinutes(d.IsAutoDeletable)).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true));
                if (Result > 0) return MessageId;
            }
            return 0;
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
                        RepliesMsgShortText = null,
                        RepliedMessageId = null,
                        DiscussionId = Model.DiscussionId,
                        UserId = Model.UserId,
                        IsPinned = Model.IsPinned,
                        IsAutoDeletable = Model.IsAutoDeletable,
                        SentAt = Model.SentAt.Value,                     
                        Text = Model.Text,
                        IsDeleted = false,
                        IsEdited = false,
                    };
                    await _context.AddAsync(discussionMessage);
                    await _context.SaveChangesAsync();
                    await _context.Discussions.AsNoTracking().Where(d => d.Id == Model.DiscussionId).ExecuteUpdateAsync(d => d.SetProperty(d => d.LastMessageId, discussionMessage.Id));

                    return discussionMessage.Id;
                }
            }
            return 0;
        }

        public async override Task<int> ReplyToMessageAsync(SendMessage Model)
        {
            if(!String.IsNullOrWhiteSpace(Model.Text) && !String.IsNullOrWhiteSpace(Model.ReplyText) && Model.UserId != 0 && Model.MessageId != 0 && Model.DiscussionId != 0)
            {
                bool IsThisUserActive = await _context.DiscussionUsers.AsNoTracking().AnyAsync(d => d.DiscussionId == Model.DiscussionId && d.UserId == Model.UserId && !d.IsBlocked && !d.IsDeleted);
                if (IsThisUserActive)
                {
                    Model.ReplyText = Model.ReplyText.Length > 40 ? Model.ReplyText.Substring(0, 37) + "..." : Model.ReplyText;

                    DiscussionMessage discussionMessage = new DiscussionMessage
                    {
                        Text = Model.Text,
                        RepliesMsgShortText = Model.ReplyText,
                        DiscussionId = Model.DiscussionId,
                        IsAutoDeletable = 0,
                        IsChecked = false,
                        IsDeleted = false,
                        IsEdited = false,
                        IsPinned = false,
                        RepliedMessageId = Model.MessageId,
                        SentAt = DateTime.Now,
                        UserId = Model.UserId
                    };

                    await _context.AddAsync(discussionMessage);
                    await _context.SaveChangesAsync();

                    return discussionMessage.Id;
                }
                else return -128;
            }
            return 0;
        }

        public override async Task<int> SentMessagesCountAsync(int DiscussionId)
        {
            if (DiscussionId != 0) return await _context.DiscussionMessages.AsNoTracking().CountAsync(d => d.DiscussionId == DiscussionId && !d.IsDeleted);
            else return 0;
        }

        public async override Task<int> GetMessageRepliesCountAsync(int Id)
        {
            if (Id > 0) return await _context.DiscussionMessages.AsNoTracking().CountAsync(d => d.RepliedMessageId == Id && !d.IsDeleted && d.RepliedMessageId != null);
            else return 0;
        }

        public async override Task<int> ScheduleMessageAsync(SendMessage Model)
        {
            if (!String.IsNullOrWhiteSpace(Model.Text) && Model.UserId != 0 && Model.SentAt.HasValue)
            {
                ScheduledMessage scheduledMessage = new ScheduledMessage
                {
                    Text = Model.Text,
                    UserId = Model.UserId,
                    IsAutoDeletable = false,
                    IsDeleted = false,
                    ScheduledTime = Model.SentAt.Value,
                    DiscussionId = Model.DiscussionId
                };
                await _context.AddAsync(scheduledMessage);
                await _context.SaveChangesAsync();

                return scheduledMessage.Id;
            }
            else return 0;
        }

        public override IQueryable<ScheduledMessage>? GetScheduledMessages(int UserId)
        {
            if (UserId != 0) return _context.ScheduledMessages.AsNoTracking().Where(scd => scd.UserId == UserId && !scd.IsDeleted).Select(d => new ScheduledMessage { Id = d.Id, IsAutoDeletable = d.IsAutoDeletable, Text = d.Text, ScheduledTime = d.ScheduledTime }).OrderByDescending(d => d.ScheduledTime);
            else return null;
        }

        public async override Task<int> TryToSendScheduledMessagesAsync(int DiscussionId)
        {
            if(DiscussionId > 0)
            {
                List<ScheduledMessage>? MessageInfos = await _context.ScheduledMessages.AsNoTracking().Where(d => d.DiscussionId == DiscussionId && !d.IsDeleted).Select(d => new ScheduledMessage { DiscussionId = DiscussionId, Text = d.Text, IsAutoDeletable = d.IsAutoDeletable, ScheduledTime = d.ScheduledTime, UserId = d.UserId}).ToListAsync();
                if(MessageInfos.Count > 0)
                {
                    List<DiscussionMessage>? ScheduledMessages = new List<DiscussionMessage>();
                    foreach(ScheduledMessage message in MessageInfos)
                    {
                        DiscussionMessage discussionMessage = new DiscussionMessage
                        {
                            Text = message.Text,
                            DiscussionId = message.DiscussionId,
                            IsAutoDeletable = 0,
                            IsDeleted = false,
                            IsEdited = false,
                            IsChecked = false,
                            RepliedMessageId = null,
                            RepliesMsgShortText = null,
                            IsPinned = false,
                            SentAt = message.ScheduledTime,
                            UserId = message.UserId
                        };
                        ScheduledMessages.Append(discussionMessage);

                        await _context.AddRangeAsync(ScheduledMessages);
                        await _context.SaveChangesAsync();

                        return ScheduledMessages.Count;
                    }
                }
            }
            return 0;
        }
    }
}
