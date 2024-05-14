using AppY.Models;
using AppY.ViewModels;

namespace AppY.Abstractions
{
    public abstract class ChatMessageAbstraction : Message
    {
        public abstract Task<(string?, string?)> SendMessageWImagesAsync(SendMessage Model);
        public abstract Task<(string?, string?)> ReplyWImagesAsync(SendReply Model);
        public abstract Task<int> SendImagesAsync(IFormFileCollection Files, int MessageId, int UserId);
        public abstract Task<string?> Forward(ForwardMessage Model);
        public abstract IQueryable<ChatMessage>? GetMessagesShortly(int Id, int SkipCount, int LoadCount);
    }
}
