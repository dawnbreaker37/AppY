using AppY.Abstractions;
using AppY.Data;
using AppY.Interfaces;
using AppY.Repositories;
using AppY.ViewModels;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using static System.Net.Mime.MediaTypeNames;

namespace AppY.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly Context _context;
        private readonly IChat _chat;
        private readonly ChatMessageAbstraction _message;

        public ChatHub(Context context, ChatMessageAbstraction message, IChat chat)
        {
            _context = context;
            _chat = chat;
            _message = message;
        }

        public async Task EditChat(int Id, string ReceiverId, string? Name, string? Description, string? Shortname, int CurrentChatId)
        {
            Chat_ViewModel Model = new Chat_ViewModel
            {
                Name = Name,
                Description = Description,
                Shortname = Shortname,
                Id = Id
            };

            int Result = await _chat.EditChatInfoAsync(Model);
            if(Result > 0)
            {
                await this.Clients.Caller.SendAsync("Caller_EditChat", Name, Description, Shortname);
                await this.Clients.User(ReceiverId).SendAsync("EditChat", Name, Description, Shortname, CurrentChatId);
            }
        }

        public async Task ClearHistory(int Id, int UserId, string ReceiverId, int ChatId)
        {
            int Result = await _chat.ClearChatHistoryAsync(Id, UserId);
            if(Result > 0)
            {
                await this.Clients.Caller.SendAsync("Caller_ClearHistory");
                await this.Clients.User(ReceiverId).SendAsync("ClearHistory", ChatId);
            }
        }

        public async Task Delete(int Id, int UserId, string ReceiverId, int ChatId)
        {
            int Result = await _chat.DeleteChatAsync(Id, UserId);
            if (Result > 0)
            {
                await this.Clients.Caller.SendAsync("Caller_DeleteChat");
                await this.Clients.User(ReceiverId).SendAsync("DeleteChat", ChatId);
            }
        }

        public async Task Typing(string ReceiverId)
        {
            await this.Clients.User(ReceiverId).SendAsync("Typing");
        }

        public async Task Idling(string ReceiverId)
        {
            await this.Clients.User(ReceiverId).SendAsync("Idling");
        }

        public async Task ClosedTheChat(string ReceiverId)
        {
            await this.Clients.User(ReceiverId).SendAsync("ClosedTheChat", DateTime.Now);
        }

        public async Task OpenedTheChat(string ReceiverId)
        {
            await this.Clients.User(ReceiverId).SendAsync("OpenedTheChat");
        }

        public async Task SendFromAlert(string? Text, int SenderId, int ReceiverId, int ChatId, int IsAutodeletable, string? Chatname, int CurrentChatUserId)
        {
            await Send(Text, SenderId, ReceiverId, ChatId, 0, Chatname, CurrentChatUserId);
        }

        public async Task Send(string? Text, int SenderId, int ReceiverId, int ChatId, int IsAutodeletable, string? Chatname, int CurrentChatUserId)
        {
            SendMessage Model = new SendMessage()
            {
                Text = Text,
                SentAt = DateTime.Now,
                ChatId = ChatId,
                IsAutoDeletable = IsAutodeletable,
                UserId = SenderId,
                DiscussionId = ChatId,
                CurrentChatUserId = CurrentChatUserId
            };
            string? Result = await _message.SendMessageAsync(Model);
            if (Result != null)
            {
                bool IsChatMuted = await _chat.IsChatMutedAsync(ChatId, ReceiverId);
                await this.Clients.User(ReceiverId.ToString()).SendAsync("Receive", Text, Result, ChatId, Chatname, SenderId, IsChatMuted);
                await this.Clients.Caller.SendAsync("CallerReceive", Text, Result, ChatId);
            }
            else await this.Clients.All.SendAsync("Error", "Can't send this message now");
        }

        public async Task Forward(int MessageId, int ToChatId, int FromChatId, string? Caption, string? ForwardingText, int UserId, int CurrentChatUserId)
        {
            ForwardMessage forwardMessage = new ForwardMessage
            {
                Caption = Caption,
                ForwardingText = ForwardingText,
                FromChatId = FromChatId,
                ToChatId = ToChatId,
                MessageId = MessageId,
                CurrentChatUserId = CurrentChatUserId,
                UserId = UserId
            };
            string? Result = await _message.Forward(forwardMessage);
            if(Result != null)
            {
                int ReceiverId = await _chat.ChatSecondUserIdAsync(ToChatId, UserId);
                await this.Clients.User(ReceiverId.ToString()).SendAsync("ReplyReceive", Result, Caption, ForwardingText, MessageId, ToChatId);
                await this.Clients.Caller.SendAsync("Forward_Success");
            }
            else
            {
                await this.Clients.Caller.SendAsync("ForwardDeny");
            }
        }

        public async Task Reply(int MessageId, string? ReplyText, int UserId, int ChatId, string? Text, int IsAutodeletable, int ReceiverId, int CurrentChatUserId)
        {
            SendReply sendReply = new SendReply
            {
                ChatId = ChatId,
                MessageId = MessageId,
                ReplyText = ReplyText,
                UserId = UserId,
                IsAutoDeletable = IsAutodeletable,
                CurrentChatUserId = CurrentChatUserId,
                Text = Text
            };
            string? Result = await _message.ReplyToMessageAsync(sendReply);
            if (Result != null)
            {
                await this.Clients.User(ReceiverId.ToString()).SendAsync("ReplyReceive", Result, Text, ReplyText, MessageId, ChatId);
                await this.Clients.Caller.SendAsync("Caller_ReplyReceive", Result, Text, ReplyText, MessageId);
            }
        }

        public async Task Edit(int Id, int UserId, int ReceiverId, string? Text, int ChatId)
        {
            SendEdit Model = new SendEdit
            {
                Id = Id,
                Text = Text,
                UserId = UserId,
                DiscussionId = ChatId
            };
            int Result = await _message.EditMessageAsync(Model);
            if(Result > 0)
            {
                await _message.EditAllRepliedMessageTextsAsync(Id, Text);
                await this.Clients.Caller.SendAsync("Edit_CallerReceive", Text, Result);
                await this.Clients.User(ReceiverId.ToString()).SendAsync("EditReceive", Text, Result, ChatId);
            }
        }

        public async Task DeleteMessage(int Id, int UserId, int ChatId, string ReceiverId)
        {
            int Result = await _message.DeleteMessageAsync(Id, UserId, ChatId);
            if(Result > 0)
            {
                await this.Clients.Caller.SendAsync("Caller_DeleteMessage", Id);
                await this.Clients.User(ReceiverId).SendAsync("DeleteMessage", Id, ChatId);
            }
        }

        //Secret Chats//
        public async Task SecretSend(string? Message, int SenderId, string ReceiverId, int ChatId)
        {
            await this.Clients.Caller.SendAsync("CallerSecretReceive", Message, SenderId);
            await this.Clients.User(ReceiverId).SendAsync("SecretReceive", Message, SenderId, ReceiverId, ChatId);
        }

        public async Task SecretReply(int ReplyId, string? ReplyText, string? Message, int SenderId, string ReceiverId, int ChatId)
        {
            if (ReplyText != null) ReplyText = ReplyText.Length > 50 ? ReplyText.Substring(0, 50) : ReplyText;
            else ReplyText = "Deleted Message";

            await this.Clients.Caller.SendAsync("CallerSecretReply", ReplyId, ReplyText, Message, SenderId);
            await this.Clients.User(ReceiverId).SendAsync("SecretReply", ReplyId, ReplyText, Message, ReceiverId, ChatId);
        }

        public async Task SecretEdit(int Id, string? Message, int SenderId, string ReceiverId, int ChatId)
        {
            await this.Clients.Caller.SendAsync("CallerSecretEdit", Id, Message, SenderId);
            await this.Clients.User(ReceiverId).SendAsync("SecretEdit", Id, Message, SenderId, ReceiverId, ChatId);
        }

        public async Task SecretDelete(int Id, int SenderId, string ReceiverId, int ChatId)
        {
            await this.Clients.Caller.SendAsync("CallerSecretDelete", Id, SenderId);
            await this.Clients.User(ReceiverId).SendAsync("SecretDelete", Id, ReceiverId, ChatId);
        }
        //Secret Chats//
    }
}
