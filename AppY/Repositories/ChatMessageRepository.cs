using AppY.Abstractions;
using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class ChatMessageRepository : ChatMessageAbstraction
    {
        private readonly Context _context;
        private readonly IChat _chat;

        public ChatMessageRepository(Context context, IChat chat)
        {
            _context = context;
            _chat = chat;
        }
        public override Task<string?> SendImagesAsync(int Id, IFormFileCollection? Files)
        {
            throw new NotImplementedException();
        }
        public async override Task<string?> SendMessageAsync(SendMessage Model)
        {
            if (!String.IsNullOrWhiteSpace(Model.Text) && Model.Text.Length <= 3400 && Model.UserId > 0 && Model.ChatId > 0)
            {
                ChatMessage chatMessage = new ChatMessage()
                {
                    Text = Model.Text,
                    UserId = Model.UserId,
                    ChatId = Model.ChatId,
                    IsAutodeletable = Model.IsAutoDeletable,
                    IsDeleted = false,
                    IsEdited = false,
                    IsPinned = false,
                    SendAt = DateTime.Now
                };
                await _context.AddAsync(chatMessage);
                await _context.SaveChangesAsync();

                return chatMessage.Id.ToString();
            }
            else return null;
        }
        public override Task<string?> ReplyToMessageAsync(SendReply Model)
        {
            throw new NotImplementedException();
        }
        public override async Task<int> EditMessageAsync(SendEdit Model)
        {
            if(!String.IsNullOrEmpty(Model.Text) && Model.Text.Length <= 3400 && Model.Id > 0 && Model.UserId > 0 && Model.DiscussionId > 0)
            {
                int Result = await _context.ChatMessages.AsNoTracking().Where(c => c.Id == Model.Id && c.UserId == Model.UserId && c.ChatId == Model.DiscussionId && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.Text, Model.Text).SetProperty(c => c.IsEdited, true));
                if (Result > 0) return Model.Id;
            }
            return 0;
        }
        public override Task<int> MarkAsReadAllMessagesAsync(int DiscussionId, int UserId)
        {
            throw new NotImplementedException();
        }
        public override Task<int> MarkAsReadAsync(int MessageId, int UserId)
        {
            throw new NotImplementedException();
        }
        public override Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId)
        {
            throw new NotImplementedException();
        }
        public override Task<DiscussionMessage?> GetPinnedMessageInfoAsync(int Id, int SkipCount)
        {
            throw new NotImplementedException();
        }
        public override Task<int> GetPinnedMessagesCountAsync(int Id)
        {
            throw new NotImplementedException();
        }
        public override Task<int> PinMessageAsync(int Id, int UserId)
        {
            throw new NotImplementedException();
        }
        public override Task<int> UnpinMessageAsync(int Id, int DiscussionOrChatId, int UserId)
        {
            throw new NotImplementedException();
        }

        public override IQueryable<IGrouping<DateTime, DiscussionMessage>>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount)
        {
            if (Id > 0 && UserId > 0) return _context.ChatMessages.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted).Select(c => new DiscussionMessage { Id = c.Id, UserId = c.UserId, IsAutoDeletable = c.IsAutodeletable, IsEdited = c.IsEdited, IsChecked = c.IsChecked, RepliedMessageId = c.RepliedMessageId, RepliesMsgShortText = c.RepliesMessageText, IsPinned = c.IsPinned, SentAt = c.SendAt, Text = c.Text }).OrderByDescending(c => c.SentAt).Skip(SkipCount).Take(LoadCount).GroupBy(c => c.SentAt.Date);
            else return null;
        }

        public override IQueryable<DiscussionMessage>? GetMessages(int Id, int SkipCount, int LoadCount)
        {
            throw new NotImplementedException();
        }
        public override async Task<int> SentMessagesCountAsync(int Id)
        {
            return await _context.ChatMessages.AsNoTracking().CountAsync(c => c.ChatId == Id && !c.IsDeleted);
        }
        public override Task<int> GetMessageRepliesCountAsync(int Id)
        {
            throw new NotImplementedException();
        }
        public async override Task<DiscussionMessage?> GetMessageInfoAsync(int Id, int UserId)
        {
            if (Id > 0 && UserId > 0) return await _context.ChatMessages.AsNoTracking().Select(c => new DiscussionMessage { Id = c.Id, UserId = c.UserId, Text = c.Text, SentAt = c.SendAt, IsEdited = c.IsEdited, IsPinned = c.IsPinned, IsChecked = c.IsChecked }).FirstOrDefaultAsync(c => c.Id == Id);
            else return null;
        }
    }
}
