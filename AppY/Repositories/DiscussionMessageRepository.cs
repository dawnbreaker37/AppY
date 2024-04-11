using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;

namespace AppY.Repositories
{
    public class DiscussionMessageRepository : Message
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DiscussionMessageRepository(Context context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _memoryCache = memoryCache;
        }

        public async override Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId)
        {
            if(Id != 0 && UserId != 0 && ChatOrDiscussionId != 0)
            {
                DateTime? NullDateVal = null;
                int Result = await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && d.UserId == UserId && d.DiscussionId == ChatOrDiscussionId && !d.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsDeleted, true).SetProperty(d => d.IsPinned, false).SetProperty(d => d.PinnedAt, NullDateVal));
                if (Result > 0)
                {
                    string? NullText = null;
                    await _context.DiscussionMessages.AsNoTracking().Where(d => d.RepliedMessageId == Id).ExecuteUpdateAsync(d => d.SetProperty(d => d.RepliesMsgShortText, NullText));
                    return Id;
                }
            }
            return 0;
        }

        public async override Task<int> EditMessageAsync(SendEdit Model)
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
                return await _context.DiscussionMessages.AsNoTracking().Where(d => d.Id == Id && !d.IsDeleted && d.UserId == UserId).Select(d => new DiscussionMessage { Text = d.Text, IsChecked = d.IsChecked, IsEdited = d.IsEdited, SentAt = d.SentAt, UserId = d.UserId, IsPinned = d.IsPinned }).FirstOrDefaultAsync();
            }
            else return null;
        }

        public override IQueryable<IGrouping<DateTime, DiscussionMessage>>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount)
        {
            if (Id != 0 && UserId != 0) return _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).Select(d => new DiscussionMessage { Id = d.Id, UserPseudoname = d.User!.PseudoName, Text = d.Text, DiscussionId = d.Id, IsAutoDeletable = d.IsAutoDeletable, IsChecked = d.IsChecked, IsEdited = d.IsEdited, SentAt = d.SentAt, UserId = d.UserId, IsPinned = d.IsPinned, RepliedMessageId = d.RepliedMessageId, RepliesMsgShortText = d.RepliesMsgShortText != null ? d.RepliesMsgShortText : "Deleted message", ImagesCount = d.DiscussionMessageImages != null ? d.DiscussionMessageImages.Count : 0, MainImgUrl = d.DiscussionMessageImages != null ? d.DiscussionMessageImages.Select(d => d.Url).FirstOrDefault() : null }).OrderByDescending(d => d.SentAt).Skip(SkipCount).Take(LoadCount).GroupBy(d => d.SentAt.Date);
            else return null;
        }

        public override IQueryable<DiscussionMessage>? GetMessages(int Id, int SkipCount, int LoadCount)
        {
            if (Id > 0) return _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted).OrderByDescending(d => d.SentAt).Skip(SkipCount).Take(LoadCount).Select(d => new DiscussionMessage { Id = d.Id, Text = d.Text, IsChecked = d.IsChecked, IsEdited = d.IsEdited, IsAutoDeletable = d.IsAutoDeletable, SentAt = d.SentAt, RepliedMessageId = d.RepliedMessageId, RepliesMsgShortText = d.RepliedMessageId > 0 ? d.RepliesMsgShortText : null, MainImgUrl = d.DiscussionMessageImages != null ? d.DiscussionMessageImages.Select(d => d.Url).FirstOrDefault() : null, UserId = d.UserId, UserPseudoname = d.User!.PseudoName });
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

        public async override Task<string?> SendMessageAsync(SendMessage Model)
        {
            if (Model.DiscussionId != 0 && Model.UserId != 0 && (Model.Images == null && !String.IsNullOrWhiteSpace(Model.Text) || Model.Images != null))
            {
                string? FirstImgUrl = null;
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
                        IsAutoDeletable = Model.IsAutoDeletable,
                        SentAt = Model.SentAt.Value,
                        Text = Model.Text,
                        IsPinned = false,
                        IsChecked = false,
                        IsDeleted = false,
                        IsEdited = false
                    };
                    await _context.AddAsync(discussionMessage);
                    await _context.SaveChangesAsync();

                    Model.Id = discussionMessage.Id;
                    //await _context.Discussions.AsNoTracking().Where(d => d.Id == Model.DiscussionId).ExecuteUpdateAsync(d => d.SetProperty(d => d.LastMessageId, discussionMessage.Id));
                    if (Model.Images != null)
                    {
                        FirstImgUrl = await SendImagesAsync(Model.Id, Model.Images);
                        return FirstImgUrl;
                    }
                    else return discussionMessage.Text;
                }
            }
            return null;
        }

        public async override Task<string?> ReplyToMessageAsync(SendReply Model)
        {
            if(!String.IsNullOrWhiteSpace(Model.Text) && !String.IsNullOrWhiteSpace(Model.ReplyText) && Model.UserId != 0 && Model.MessageId != 0 && Model.DiscussionId != 0)
            {
                string? FirstSendImg = null;
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

                    Model.Id = discussionMessage.Id;
                    Model.SentAt = DateTime.Now;

                    if (Model.Images != null)
                    {
                        FirstSendImg = await SendImagesAsync(Model.Id, Model.Images);
                        return FirstSendImg;
                    }
                    else return discussionMessage.Text;
                }
            }
            return null;
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

        public async override Task<string?> SendImagesAsync(int Id, IFormFileCollection? Files)
        {
            if(Id > 0 && Files != null)
            {
                string? FirstFileName = null;
                int MaxCount = Files.Count > 6 ? 6 : Files.Count;
                for(int i = 0; i < MaxCount; i++)
                {
                    string? FileExtension = Path.GetExtension(Files[i].FileName);
                    string? FileRandName = Guid.NewGuid().ToString().Substring(2, 14);
                    if (i == 0) FirstFileName = FileRandName + FileExtension;

                    using(FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "/DiscussionMessageImages/" + FileRandName + FileExtension, FileMode.Create))
                    {
                        await Files[i].CopyToAsync(fs);
                        DiscussionMessageImage discussionMessageImage = new DiscussionMessageImage
                        {
                            IsDeleted = false,
                            MessageId = Id,
                            Url = FileRandName + FileExtension
                        };
                        await _context.AddAsync(discussionMessageImage);
                    }
                }
                await _context.SaveChangesAsync();

                return FirstFileName;
            }
            return null;
        }

        public async override Task<int> PinMessageAsync(int Id, int UserId)
        {
            if(Id > 0 && UserId > 0)
            {
                bool CheckUsersAccess = await _context.DiscussionUsers.AsNoTracking().AnyAsync(u => u.UserId == UserId && u.AccessLevel > 0);
                if(CheckUsersAccess)
                {
                    int Result = await _context.DiscussionMessages.AsNoTracking().Where(dm => dm.Id == Id && !dm.IsDeleted).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsPinned, true).SetProperty(d => d.PinnedAt, DateTime.Now));
                    if (Result > 0) return Id;
                } 
            }
            return 0;
        }

        public async override Task<int> UnpinMessageAsync(int Id, int DiscussionOrChatId, int UserId)
        {
            if (Id > 0 && DiscussionOrChatId > 0 && UserId > 0)
            {
                bool CheckUsersAccess = await _context.DiscussionUsers.AsNoTracking().AnyAsync(u => u.UserId == UserId && u.AccessLevel > 0);
                if (CheckUsersAccess)
                {
                    DateTime? NullDate = null;
                    int Result = await _context.DiscussionMessages.AsNoTracking().Where(dm => dm.Id == Id && !dm.IsDeleted).ExecuteUpdateAsync(dm => dm.SetProperty(dm => dm.IsPinned, false).SetProperty(d => d.PinnedAt, NullDate));
                    if (Result > 0) return await _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == DiscussionOrChatId && d.IsPinned && !d.IsDeleted).Select(d => d.Id).FirstOrDefaultAsync();
                }
            }
            return -256;
        }

        public async override Task<DiscussionMessage?> GetPinnedMessageInfoAsync(int Id, int SkipCount)
        {
            if (Id > 0)
            {
                return await _context.DiscussionMessages.AsNoTracking().Where(d => d.DiscussionId == Id && !d.IsDeleted && d.IsPinned).Select(d => new DiscussionMessage { Id = d.Id, Text = d.Text, PinnedAt = d.PinnedAt }).OrderByDescending(d => d.PinnedAt).Skip(SkipCount).FirstOrDefaultAsync();
            }
            else return null;
        }
        
        public async override Task<int> GetPinnedMessagesCountAsync(int Id)
        {
            return await _context.DiscussionMessages.AsNoTracking().CountAsync(d => d.DiscussionId == Id && d.IsPinned && !d.IsDeleted);
        }
    }
}
