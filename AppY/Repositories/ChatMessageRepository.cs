﻿using AppY.Abstractions;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChatMessageRepository(Context context, IChat chat, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _chat = chat;
            _webHostEnvironment = webHostEnvironment;
        }
        public override async Task<string?> SendImagesAsync(int Id, IFormFileCollection? Files)
        {
            if(Files != null)
            {
                string? FirstFileName = null;
                int FilesCount = Files.Count > 6 ? 6 : Files.Count;
                for(int i = 0; i < FilesCount; i++)
                {
                    string? FileName = Guid.NewGuid().ToString("N").Substring(4, 14);
                    string? Extension = Path.GetExtension(Files[i].FileName);
                    if (i == 0) FirstFileName = FileName + Extension;

                    using(FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "/ChatMessageImages/" + FileName + Extension, FileMode.Create))
                    {
                        await Files[i].CopyToAsync(fs);
                        ChatMessageImage chatMessageImage = new ChatMessageImage
                        {
                            Name = FileName + Extension,
                            MessageId = Id
                        };
                        await _context.AddAsync(chatMessageImage);
                    }
                }
                await _context.SaveChangesAsync();

                return FirstFileName;
            }
            return null;
        }

        public async override Task<(string?, string?)> SendMessageWImagesAsync(SendMessage Model)
        {
            string? MessageResult = await SendMessageAsync(Model);
            if(MessageResult != null)
            {
                string? ImageResult = await SendImagesAsync(Int32.Parse(MessageResult), Model.Images);
                if (ImageResult != null) return (MessageResult, ImageResult);
            }
            return (null, null);
        }

        public async override Task<string?> SendMessageAsync(SendMessage Model)
        {
            bool CheckUser = await _context.ChatUsers.AsNoTracking().AnyAsync(u => u.ChatId == Model.ChatId && u.UserId == Model.UserId);
            if (CheckUser)
            {               
                if (!String.IsNullOrWhiteSpace(Model.Text) && Model.Text.Length <= 3400 && Model.UserId > 0 && Model.ChatId > 0)
                {
                    if(Model.MessageType > 0)
                    {
                        Model.ReplyText = Model.Text.Substring(Model.Text.IndexOf(":") + 1);
                        Model.Text = Model.Text.Substring(0, Model.Text.IndexOf(":"));
                    }

                    ChatMessage chatMessage = new ChatMessage()
                    {
                        Text = Model.Text,
                        MessageType = Model.MessageType,
                        UserId = Model.UserId,
                        ChatId = Model.ChatId,
                        RepliesMessageText =Model.ReplyText != null ? Model.ReplyText : null,
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

        public async override Task<(string?, string?)> ReplyWImagesAsync(SendReply Model)
        {
            string? Result = await ReplyToMessageAsync(Model);
            if(Result != null)
            {
                string? ImagesResult = await SendImagesAsync(Int32.Parse(Result), Model.Images);
                if (ImagesResult != null) return (Result, ImagesResult);
            }
            return (null, null);
        }

        public async override Task<string?> ReplyToMessageAsync(SendReply Model)
        {
            if ((Model.MessageId == -256 || Model.MessageId > 0) && Model.UserId > 0 && Model.CurrentChatUserId > 0 && Model.ChatId > 0 && !String.IsNullOrWhiteSpace(Model.Text) && Model.Text.Length <= 3400 && !String.IsNullOrWhiteSpace(Model.ReplyText))
            {
                bool CheckUser = await _context.ChatUsers.AsNoTracking().AnyAsync(u => u.ChatId == Model.ChatId && u.UserId == Model.UserId);
                if (CheckUser)
                {
                    Model.ReplyText = Model.ReplyText.Length > 125 ? Model.ReplyText.Substring(0, 125) + "..." : Model.ReplyText;
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
                int Result = await _context.ChatMessages.AsNoTracking().Where(c => c.Id == Model.Id && c.UserId == Model.UserId && c.ChatId == Model.DiscussionId && !c.IsDeleted && c.SentAt.AddDays(3) >= DateTime.Now).ExecuteUpdateAsync(c => c.SetProperty(c => c.Text, Model.Text).SetProperty(c => c.IsEdited, true));
                if (Result > 0) return Model.Id;
            }
            return 0;
        }
        public override async Task<int> MarkAsReadAllMessagesAsync(int DiscussionId, int UserId)
        {
            if (DiscussionId > 0 && UserId > 0) return await _context.ChatMessages.AsNoTracking().Where(c => c.UserId != UserId && !c.IsChecked).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsChecked, true));
            else return 0;
        }
        public override async Task<int> MarkAsReadAsync(int MessageId, int UserId)
        {
            if(MessageId > 0 && UserId > 0)
            {
                int Result = await _context.ChatMessages.AsNoTracking().Where(c => c.Id == MessageId && c.UserId != UserId && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsChecked, true));
                if (Result > 0) return MessageId;
            }
            return 0;
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
        public override async Task<DiscussionMessage?> GetPinnedMessageInfoAsync(int Id, int SkipCount)
        {
            return await _context.ChatMessages.AsNoTracking().Where(p => p.ChatId == Id && p.IsPinned).OrderByDescending(p => p.PinnedAt).Select(p => new DiscussionMessage { Id = p.Id, Text = p.Text }).Skip(SkipCount).FirstOrDefaultAsync();
        }
        public override async Task<int> GetPinnedMessagesCountAsync(int Id)
        {
            return await _context.ChatMessages.AsNoTracking().CountAsync(p => p.ChatId == Id && !p.IsDeleted && p.IsPinned);
        }
        public override async Task<int> PinMessageAsync(int Id, int DiscussionOrChatId, int UserId)
        {
            bool CheckUserAccess = await _chat.CheckUserAvailabilityInChat(DiscussionOrChatId, UserId);
            if(CheckUserAccess)
            {
                int Result = await _context.ChatMessages.AsNoTracking().Where(c => c.Id == Id && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsPinned, true).SetProperty(c => c.PinnedAt, DateTime.Now));
                if (Result > 0) return Id;
            }
            return 0;
        }
        public override async Task<int> UnpinMessageAsync(int Id, int DiscussionOrChatId, int UserId)
        {
            bool CheckUserAccess = await _chat.CheckUserAvailabilityInChat(DiscussionOrChatId, UserId);
            if(CheckUserAccess)
            {
                DateTime? NullDate = null;
                int Result = await _context.ChatMessages.AsNoTracking().Where(c => c.Id == Id && !c.IsDeleted).ExecuteUpdateAsync(c => c.SetProperty(c => c.IsPinned, false).SetProperty(c => c.PinnedAt, NullDate));
                if (Result > 0) return await _context.ChatMessages.AsNoTracking().Where(c => c.IsPinned && !c.IsDeleted && c.ChatId == DiscussionOrChatId).Select(c => c.Id).FirstOrDefaultAsync();
            }
            return 0;
        }

        public override IQueryable<IGrouping<DateTime, DiscussionMessage>>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount)
        {
            if (Id > 0 && UserId > 0) return _context.ChatMessages.AsNoTracking().Where(c => c.ChatId == Id && !c.IsDeleted).Select(c => new DiscussionMessage { Id = c.Id, UserId = c.UserId, MessageType = c.MessageType, IsAutoDeletable = c.IsAutodeletable, IsEdited = c.IsEdited, IsChecked = c.IsChecked, RepliedMessageId = c.RepliedMessageId, RepliesMsgShortText = c.RepliesMessageText, IsPinned = c.IsPinned, SentAt = c.SentAt, Text = c.Text }).OrderBy(c => c.SentAt).Skip(SkipCount).Take(LoadCount).GroupBy(c => c.SentAt.Date);
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
            if (Model.CurrentChatUserId > 0 && Model.ToChatId > 0 && Model.ToChatId != Model.FromChatId && Model.FromChatId > 0 && !String.IsNullOrWhiteSpace(Model.ForwardingText) && Model.ForwardingText.Length <= 3400)
            {
                ChatMessage chatMessage;
                bool ChatAvailability = await _context.ChatUsers.AsNoTracking().AnyAsync(c => c.UserId == Model.UserId && c.ChatId == Model.ToChatId);
                if (ChatAvailability)
                {
                    if (String.IsNullOrEmpty(Model.Caption))
                    {
                        chatMessage = new ChatMessage
                        {
                            ChatId = Model.ToChatId,
                            IsChecked = false,
                            IsDeleted = false,
                            IsPinned = false,
                            IsEdited = false,
                            Text = Model.ForwardingText,
                            IsAutodeletable = 0,
                            ChatUserId = Model.CurrentChatUserId,
                            RepliedMessageId = 0,
                            RepliesMessageText = null,
                            SentAt = DateTime.Now,
                            UserId = Model.UserId
                        };
                    }
                    else
                    {
                        Model.ForwardingText = Model.ForwardingText.Length > 125 ? Model.ForwardingText.Substring(0, 125) + "..." : Model.ForwardingText;
                        chatMessage = new ChatMessage
                        {
                            ChatId = Model.ToChatId,
                            IsChecked = false,
                            IsDeleted = false,
                            IsPinned = false,
                            IsEdited = false,
                            Text = Model.Caption,
                            IsAutodeletable = 0,
                            ChatUserId = Model.CurrentChatUserId,
                            RepliedMessageId = -256,
                            RepliesMessageText = Model.ForwardingText,
                            SentAt = DateTime.Now,
                            UserId = Model.UserId
                        };
                    }

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

        public async override Task<bool> IsPinnedAsync(int Id)
        {
            return await _context.ChatMessages.AsNoTracking().AnyAsync(c => c.Id == Id && !c.IsDeleted && c.IsPinned);
        }

        public override Task<int> SendImagesAsync(IFormFileCollection Files, int MessageId, int UserId)
        {
            throw new NotImplementedException();
        }
    }
}
