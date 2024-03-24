using AppY.Controllers;
using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IDiscussion
    {
        public Task<int> CreateDiscussionAsync(Discussion_ViewModel Model);
        public Task<int> DeleteDiscussionAsync(int Id, int UserId, string? DiscussionName);
        public Task<int> RestoreDiscussionAsync(int Id, int UserId);
        public Task<int> AddMemberAsync(int Id, int AdderId, int UserId);
        public Task<int> JoinAsync(int Id, int UserId);
        public Task<int> JoinToPrivateAsync(int Id, int UserId, string? Password);
        public Task<bool> CheckDiscussionPasswordAsync(int Id, string? Password);
        public Task<int> LeaveAsync(int Id, int UserId);
        public Task<int> DeleteUserAsync(int Id, int DeleterId, int UserId);
        public Task<bool> EditDiscussionAsync(Discussion_ViewModel Model);
        public Task<int> MuteAsync(int Id, int UserId);
        public Task<int> UnmuteAsync(int Id, int UserId);
        public Task<int> PinAsync(int Id, int UserId);
        public Task<int> UnpinAsync(int Id, int UserId);
        public Task<int> BlockUserAsync(int Id, int BlockerId, int UserId);
        public Task<int> UnblockUserAsync(int Id, int UnblockerId, int UserId);
        public Task<int> ChangeAccessLevel(int Id, int ChangerId, int UserId, int AccessLevel);
        public IQueryable<DiscussionShortInfo?>? GetUserMessagesSortedByDiscussions(int Id);
        public IQueryable<DiscussionShortInfo>? GetUserDiscussions(int Id);
        public IQueryable<DiscussionShortInfo>? GetUserDeletedDiscussions(int Id);
        public IQueryable<DiscussionUsers>? GetMembersInfo(int Id);
        public IQueryable<DiscussionShortInfo>? Find(string? Keyword);
        public Task<int> GetDiscussionIdByShortlinkAsync(string Shortlink);
        public Task<Discussion?> GetDiscussionInfoAsync(int Id);
        public Task<Discussion?> GetDiscussionInfoAsync(string Id);
        public Task<Discussion?> GetDiscussionShortInfoAsync(int Id, int UserId);
        public Task<int> GetMembersCountAsync(int Id);
        public Task<int> GetDeletedDiscussionsCountAsync(int Id);
        public Task<bool> HasThisUserAccessToThisDiscussionAsync(int UserId, int DiscussionId);
        public Task<bool> IsShortLinkFreeAsync(int Id, string? Shortlink);
        public Task<bool> IsThisDiscussionMutedAsync(int Id, int UserId);
    }
}
