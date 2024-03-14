using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;

namespace AppY.Abstractions
{
    public abstract class Message
    {
        public abstract Task<int> SendMessageAsync(SendMessage Model);
        public abstract Task<int> EditMessageAsync(SendMessage Model);
        public abstract Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId);
        public abstract IQueryable<DiscussionMessage>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount);
        public abstract Task<int> SentMessagesCountAsync(int DiscussionId);
        public abstract Task<DiscussionMessage?> GetMessageInfoAsync(int Id, int UserId);
    }
}
