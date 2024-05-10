using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class SavedMessageRepository : Base<SavedMessageContent>, ISavedMessage
    {
        private readonly Context _context;
        private readonly IChat _chat;
        private readonly IDiscussion _discussion;
        public SavedMessageRepository(Context context, IChat chat, IDiscussion discussion) : base(context)
        {
            _context = context;
            _chat = chat;
            _discussion = discussion;
        }

        public async Task<int> AddSavedMessageAsync(SavedMessageContent_ViewModel Model)
        {
            if(Model.UserId > 0)
            {
                SavedMessageContent savedMessageContent = new SavedMessageContent()
                {
                    Addition = Model.Addition,
                    Text = Model.Text,
                    UserId = Model.UserId,
                    SentAt = DateTime.Now,
                    Badge = Model.Badge,
                    ChatMessageId = Model.ChatId > 0 ? Model.MessageId : null,
                    DiscussionMessageId = Model.DiscussionId > 0 ? Model.MessageId : null,
                    IsEdited = false,
                    IsPinned = false,
                    IsDeleted = false,
                };
                await _context.AddAsync(savedMessageContent);
                await _context.SaveChangesAsync();

                return savedMessageContent.Id;
            }
            return 0;
        }

        public async Task<int> DeleteSavedMessageAsync(int Id, int UserId)
        {
            int Result = await _context.SavedMessagesContent.AsNoTracking().Where(s => s.Id == Id && s.UserId == UserId).ExecuteUpdateAsync(s => s.SetProperty(s => s.IsDeleted, true).SetProperty(s => s.IsPinned, false));
            if (Result > 0) return Id;
            else return 0;
        }

        public async Task<string?> EditSavedMessageAsync(SavedMessageContent_ViewModel Model)
        {
            if(Model.UserId > 0 && Model.MessageId > 0 && !String.IsNullOrWhiteSpace(Model.Text))
            {
                int Result = await _context.SavedMessagesContent.AsNoTracking().Where(s => s.Id == Model.MessageId && !s.IsDeleted).ExecuteUpdateAsync(s => s.SetProperty(s => s.Text, Model.Text).SetProperty(s => s.IsEdited, true));
                if (Result > 0) return Model.Text;
            }
            return null;
        }

        public async Task<SavedMessageContent?> GetPinnedMessageInfoAsync(int Id, int SkipCount)
        {
            return await _context.SavedMessagesContent.AsNoTracking().Where(s => s.UserId == Id && !s.IsDeleted && s.IsPinned).OrderByDescending(s => s.PinnedAt!.Value).Skip(SkipCount).Select(s => new SavedMessageContent { Id = s.Id, Text = s.Text, Badge = s.Badge }).FirstOrDefaultAsync();
        }

        public IQueryable<SavedMessageContent>? GetSavedMessages(int UserId, int SkipCount, int LoadCount)
        {
            if (UserId > 0)
            {
                LoadCount = LoadCount <= 0 ? 50 : LoadCount;
                return _context.SavedMessagesContent.AsNoTracking().Where(s => s.UserId == UserId && !s.IsDeleted).OrderBy(s => s.SentAt).Skip(SkipCount).Take(LoadCount).Select(s => new SavedMessageContent { Id = s.Id, Text = s.Text, Badge = s.Badge, IsEdited = s.IsEdited, SentAt = s.SentAt, IsPinned = s.IsPinned });
            }
            else return null;
        }

        public async Task<int> GetSavedMessagesCountAsync(int Id)
        {
            return await _context.SavedMessagesContent.AsNoTracking().CountAsync(s => s.UserId == Id && !s.IsDeleted);
        }

        public async Task<bool> IsSavedMessagePinnedAsync(int Id, int UserId)
        {
            return await _context.SavedMessagesContent.AsNoTracking().AnyAsync(s => s.Id == Id && s.IsPinned && s.UserId == UserId);
        }

        public async Task<int> PinAsync(int Id, int UserId)
        {
            int Result = await _context.SavedMessagesContent.AsNoTracking().Where(s => s.Id == Id && s.UserId == UserId && !s.IsDeleted).ExecuteUpdateAsync(s => s.SetProperty(s => s.IsPinned, true).SetProperty(s => s.PinnedAt, DateTime.Now));
            if (Result > 0) return Id;
            else return 0;
        }

        public async Task<int> PinnedMessagesCountAsync(int Id)
        {
            return await _context.SavedMessagesContent.AsNoTracking().CountAsync(c => c.UserId == Id && c.IsPinned);
        }

        public async Task<bool> SaveTheMessageAsync(int Id, int ChatOrDiscussionId, int UserId, bool IsFromChat)
        {
            bool CheckUserAccessAvailability = false;
            if (IsFromChat)
            {
                CheckUserAccessAvailability = await _chat.CheckUserAvailabilityInChat(ChatOrDiscussionId, UserId);
                if (CheckUserAccessAvailability)
                {
                    string? GetMessageTextIfItIsPossible = await _context.ChatMessages.AsNoTracking().Where(c => c.Id == Id && !c.IsDeleted).Select(c => c.Text).FirstOrDefaultAsync();
                    if (!String.IsNullOrWhiteSpace(GetMessageTextIfItIsPossible))
                    {
                        SavedMessageContent_ViewModel Model = new SavedMessageContent_ViewModel
                        {
                            ChatId = ChatOrDiscussionId,
                            MessageId = Id,
                            Text = GetMessageTextIfItIsPossible,
                            UserId = UserId,
                            Badge = "Saved from Chat",
                            DiscussionId = 0,
                        };
                        int Result = await AddSavedMessageAsync(Model);
                        if (Result > 0) return true;
                    }
                }
            }
            else
            {
                CheckUserAccessAvailability = await _discussion.HasThisUserAccessToThisDiscussionAsync(UserId, ChatOrDiscussionId);
                if (CheckUserAccessAvailability)
                {
                    string? GetMessageTextIfItIsPossible = await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && !d.IsDeleted).Select(d => d.Text).FirstOrDefaultAsync();
                    if (!String.IsNullOrWhiteSpace(GetMessageTextIfItIsPossible))
                    {
                        SavedMessageContent_ViewModel Model = new SavedMessageContent_ViewModel
                        {
                            ChatId = 0,
                            MessageId = Id,
                            Text = GetMessageTextIfItIsPossible,
                            UserId = UserId,
                            Badge = "Saved from Discussion",
                            DiscussionId = ChatOrDiscussionId
                        };
                        int Result = await AddSavedMessageAsync(Model);
                        if (Result > 0) return true;
                    }
                }
            }
            return false;
        }

        public async Task<string?> StarSavedMessageAsync(int Id, string? Text)
        {
            if (Text?.Length > 12) Text = Text?.Substring(12);
            Text = Text == null ? "Starred" : Text;

            int Result = await _context.SavedMessagesContent.AsNoTracking().Where(s => s.Id == Id && !s.IsDeleted).ExecuteUpdateAsync(s => s.SetProperty(s => s.Badge, Text));
            if (Result > 0) return Text;
            else return null;
        }

        public async Task<string?> UnpinAsync(int Id, int UserId)
        {
            DateTime? NullDate = null;

            int Result = await _context.SavedMessagesContent.AsNoTracking().Where(s => s.Id == Id && s.UserId == UserId && !s.IsDeleted).ExecuteUpdateAsync(s => s.SetProperty(s => s.IsPinned, false).SetProperty(s => s.PinnedAt, NullDate));
            if (Result > 0) return await _context.SavedMessagesContent.AsNoTracking().Where(s => s.UserId == UserId && s.IsPinned).Select(s => s.Text).FirstOrDefaultAsync();
            else return null;
        }

        public async Task<bool> UnstarSavedMessageAsync(int Id)
        {
            string? NullText = null;
            int Result = await _context.SavedMessagesContent.AsNoTracking().Where(s => s.Id == Id && !s.IsDeleted).ExecuteUpdateAsync(s => s.SetProperty(s => s.Badge, NullText));
            if (Result > 0) return true;
            else return false;
        }
    }
}
