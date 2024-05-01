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
            bool CheckUser = await _context.ChatUsers.AsNoTracking().AnyAsync(u => u.ChatId == Model.DiscussionId && u.UserId == Model.UserId);
            if (CheckUser)
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
                        ChatUserId = Model.CurrentChatUserId,
                        IsEdited = false,
                        IsPinned = false,
                        SentAt = DateTime.Now
                    };
                    await _context.AddAsync(chatMessage);
                    await _context.SaveChangesAsync();

                    return chatMessage.Id.ToString();
                }
            }
            return null;
        }

        public async override Task<string?> ReplyToMessageAsync(SendReply Model)
        {
            if (Model.UserId > 0 && Model.CurrentChatUserId > 0 && Model.ChatId > 0 && !String.IsNullOrWhiteSpace(Model.Text) && Model.Text.Length <= 3400 && !String.IsNullOrWhiteSpace(Model.ReplyText) && Model.MessageId > 0)
            {
                bool CheckUser = await _context.ChatUsers.AsNoTracking().AnyAsync(u => u.ChatId == Model.ChatId && u.UserId == Model.UserId);
                if (CheckUser)
                {
                    Model.ReplyText = Model.ReplyText.Length > 40 ? Model.ReplyText.Substring(0, 37) + "..." : Model.ReplyText;
                    ChatMessage chatMessage = new ChatMessage
                    {
                        ChatId = Model.ChatId,
                        Text = Model.Text,
                        RepliedMessageId = Model.MessageId,
                        RepliesMessageText = Model.ReplyText,
                        IsChecked = false,
                        IsDeleted = false,
                        ChatUserId = Model.CurrentChatUserId,
                        SentAt = DateTime.Now,
                        IsAutodeletable = Model.IsAutoDeletable,
                        IsEdited = false,
                        IsPinned = false,
                        UserId = Model.UserId
                    };
                    await _context.AddAsync(chatMessage);
                    await _context.SaveChangesAsync();

                    return chatMessage.Id.ToString();
                }
            }
            return null;
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
        public async override Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId)
        {
            if (Id > 0 && UserId > 0 && ChatOrDiscussionId > 0)
            {
                await _context.ChatMessages.AsNoTracking().Where(c => c.Id == Id && c.UserId == UserId && c.ChatId == ChatOrDiscussionId).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsDeleted, true));
                return Id;
            }
            else return 0;
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
            if (Id > 0 && UserId > 0) return _context.ChatMessages.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted).Select(c => new DiscussionMessage { Id = c.Id, UserId = c.UserId, IsAutoDeletable = c.IsAutodeletable, IsEdited = c.IsEdited, IsChecked = c.IsChecked, RepliedMessageId = c.RepliedMessageId, RepliesMsgShortText = c.RepliesMessageText, IsPinned = c.IsPinned, SentAt = c.SentAt, Text = c.Text }).OrderBy(c => c.SentAt).Skip(SkipCount).Take(LoadCount).GroupBy(c => c.SentAt.Date);
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
            if (Id > 0 && UserId > 0) return await _context.ChatMessages.AsNoTracking().Select(c => new DiscussionMessage { Id = c.Id, UserId = c.UserId, Text = c.Text, SentAt = c.SentAt, IsEdited = c.IsEdited, IsPinned = c.IsPinned, IsChecked = c.IsChecked }).FirstOrDefaultAsync(c => c.Id == Id);
            else return null;
        }

        public async override Task<string?> Forward(ForwardMessage Model)
        {
            if(Model.MessageId > 0 && Model.CurrentChatUserId > 0 && Model.ToChatId > 0 && Model.ToChatId != Model.FromChatId && Model.FromChatId > 0 && !String.IsNullOrWhiteSpace(Model.ForwardingText) && Model.ForwardingText.Length <= 3400)
            {
                bool ChatAvailability = await _context.ChatUsers.AsNoTracking().AnyAsync(c => c.UserId == Model.UserId && c.ChatId == Model.ToChatId);
                if (ChatAvailability)
                {
                    Model.ForwardingText = Model.ForwardingText.Length > 40 ? Model.ForwardingText.Substring(0, 37) + "..." : Model.ForwardingText;

                    ChatMessage chatMessage = new ChatMessage
                    {
                        ChatId = Model.ToChatId,
                        IsChecked = false,
                        IsDeleted = false,
                        IsPinned = false,
                        IsEdited = false,
                        Text = Model.Caption,
                        IsAutodeletable = 0,
                        ChatUserId = Model.CurrentChatUserId,
                        RepliedMessageId = Model.MessageId,
                        RepliesMessageText = Model.ForwardingText,
                        SentAt = DateTime.Now,
                        UserId = Model.UserId
                    };
                    await _context.AddAsync(chatMessage);
                    await _context.SaveChangesAsync();

                    return chatMessage.Id.ToString();
                }
            }
            return null;
        }

        public override IQueryable<ChatMessage>? GetMessagesShortly(int Id, int SkipCount, int LoadCount)
        {
            if (Id > 0) return _context.ChatMessages.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted).Select(c => new ChatMessage { Text = c.Text, RepliedMessageId = c.RepliedMessageId, RepliesMessageText = c.RepliesMessageText, SentAt = c.SentAt, IsChecked = c.IsChecked, IsEdited = c.IsEdited, UserId = c.UserId }).OrderBy(c => c.SentAt).Skip(SkipCount).Take(LoadCount);
            else return null;
        }

        public async override Task<int> EditAllRepliedMessageTextsAsync(int Id, string? Text)
        {
            if (Text != null) Text = Text.Length >= 40 ? Text.Substring(0, 40) : Text;
            else Text = "Empty Text";

            int Result = await _context.ChatMessages.AsNoTracking().Where(d => d.RepliedMessageId == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.RepliesMessageText, Text));
            if (Result > 0) return Id;
            else return 0;
        }
    }
}
