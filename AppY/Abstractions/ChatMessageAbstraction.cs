using AppY.Models;
using AppY.ViewModels;

namespace AppY.Abstractions
{
    public abstract class ChatMessageAbstraction : Message
    {
        public abstract Task<string?> Forward(ForwardMessage Model);
        public abstract IQueryable<ChatMessage>? GetMessagesShortly(int Id, int SkipCount, int LoadCount);
    }
}
