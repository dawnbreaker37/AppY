using AppY.Models;
using AppY.ViewModels;

namespace AppY.Abstractions
{
    public abstract class Message
    {
        public abstract Task<string?> SendImagesAsync(int Id, IFormFileCollection? Files);
        public abstract Task<string?> SendMessageAsync(SendMessage Model);
        public abstract Task<string?> ReplyToMessageAsync(SendReply Model);
        public abstract Task<int> EditMessageAsync(SendEdit Model);
        public abstract Task<int> EditAllRepliedMessageTextsAsync(int Id, string? Text);
        public abstract Task<int> MarkAsReadAllMessagesAsync(int DiscussionId, int UserId);
        public abstract Task<int> MarkAsReadAsync(int MessageId, int UserId);
        public abstract Task<int> DeleteMessageAsync(int Id, int UserId, int ChatOrDiscussionId);
        public abstract Task<bool> IsPinnedAsync(int Id);
        public abstract Task<DiscussionMessage?> GetPinnedMessageInfoAsync(int Id, int SkipCount);
        public abstract Task<int> GetPinnedMessagesCountAsync(int Id);
        public abstract Task<int> PinMessageAsync(int Id, int DiscussionOrChatId, int UserId);
        public abstract Task<int> UnpinMessageAsync(int Id, int DiscussionOrChatId, int UserId);
        public abstract IQueryable<IGrouping<DateTime, DiscussionMessage>>? GetMessages(int Id, int UserId, int SkipCount, int LoadCount);
        public abstract IQueryable<DiscussionMessage>? GetMessages(int Id, int SkipCount, int LoadCount);
        public abstract Task<int> SentMessagesCountAsync(int DiscussionId);
        public abstract Task<int> GetMessageRepliesCountAsync(int Id);
        public abstract Task<DiscussionMessage?> GetMessageInfoAsync(int Id, int UserId);
    }
}
