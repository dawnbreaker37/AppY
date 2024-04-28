using AppY.Models;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IChat
    {
        public Task<int> CreateChatAsync(Chat_ViewModel Model);
        public Task<int> EditChatInfoAsync(Chat_ViewModel Model);
        public Task<int> ClearChatHistoryAsync(int Id, int UserId);
        public Task<int> DeleteChatAsync(int Id, int UserId);
        public Task<int> RestoreChatAsync(int Id, int UserId);
        public Task<int> PinTheChatAsync(int Id, int UserId);
        public Task<int> UnpinTheChatAsync(int Id, int UserId);
        public Task<bool> IsChatMutedAsync(int Id, int UserId);
        public Task<int> MuteTheChatAsync(int Id, int UserId);
        public Task<int> UnmuteTheChatAsync(int Id, int UserId);
        public Task<bool> CheckShortnameAvailability(int Id, string? Shortname);
        public Task<bool> CheckUserAvailabilityInChat(int Id, int UserId);
        public Task<Chat?> GetChatInfoAsync(int Id);
        public Task<int> ChatSecondUserIdAsync(int Id, int UserId);
        public Task<List<int>?> ChatUserIdsAsync(int Id);
        public IQueryable<ChatUsers>? GetUserChats(int Id);
        public IQueryable<ChatUsers>? GetUserChatsShortly(int Id, int CurrentChatId);
        public Task<int> FindChatAvailability(int User1, int User2, bool CreateIfNot);
    }
}
