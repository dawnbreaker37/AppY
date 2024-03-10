using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IDiscussionMessage
    {
        public Task<int> SendMessageAsync(SendMessage Model);
        public Task<int> SentMessagesCountAsync(int DiscussionId);
    }
}
