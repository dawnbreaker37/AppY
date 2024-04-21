using AppY.Abstractions;
using AppY.Data;
using AppY.Interfaces;
using AppY.Repositories;
using AppY.ViewModels;
using Microsoft.AspNetCore.SignalR;

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

        public async Task Send(string? Text, int SenderId, string ReceiverId, int ChatId, int IsAutodeletable)
        {
            SendMessage Model = new SendMessage()
            {
                Text = Text,
                SentAt = DateTime.Now,
                ChatId = ChatId,
                IsAutoDeletable = IsAutodeletable,
                UserId = SenderId,
                DiscussionId = ChatId
            };
            string? Result = await _message.SendMessageAsync(Model);
            if (Result != null)
            {
                await this.Clients.User(ReceiverId).SendAsync("Receive", Text, Result, ChatId);
                await this.Clients.Caller.SendAsync("CallerReceive", Text, Result);
            }
            else await this.Clients.All.SendAsync("Error", "Can't send this message now");
        }

        public async Task Edit(int Id, int UserId, string ReceiverId, string? Text, int ChatId)
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
                await this.Clients.Caller.SendAsync("Edit_CallerReceive", Text, Result);
                await this.Clients.User(ReceiverId).SendAsync("EditReceive", Text, Result, ChatId);
            }
        }
    }
}
