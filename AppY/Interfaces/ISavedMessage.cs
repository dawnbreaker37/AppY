using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface ISavedMessage : IBase<SavedMessageContent>
    {
        public Task<int> PinAsync(int Id, int UserId);
        public Task<int> UnpinAsync(int Id, int UserId);
        public Task<bool> SaveTheMessageAsync(int Id, int ChatOrDiscussionId, int UserId, bool IsFromChat);
        public Task<int> GetSavedMessagesCountAsync(int Id);
        public Task<bool> IsSavedMessagePinnedAsync(int Id, int UserId);
        public IQueryable<SavedMessageContent>? GetSavedMessages(int UserId, int SkipCount, int LoadCount);
        public Task<int> AddSavedMessageAsync(SavedMessageContent_ViewModel Model);
        public Task<string?> EditSavedMessageAsync(SavedMessageContent_ViewModel Model);
        public Task<string?> StarSavedMessageAsync(int Id, string? Text);
        public Task<bool> UnstarSavedMessageAsync(int Id);
        public Task<int> DeleteSavedMessageAsync(int Id, int UserId);
    }
}
