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
        public Task<bool> EditDiscussionAsync(Discussion_ViewModel Model);
        public Task<int> MuteAsync(int Id, int UserId);
        public Task<int> UnmuteAsync(int Id, int UserId);
        public IQueryable<DiscussionShortInfo>? GetUserDiscussions(int Id);
        public IQueryable<DiscussionShortInfo>? GetUserDeletedDiscussions(int Id);
        public IQueryable<DiscussionUsers>? GetMembersInfo(int Id);
        public Task<Discussion?> GetDiscussionInfoAsync(int Id);
        public Task<int> GetMembersCountAsync(int Id);
        public Task<int> GetDeletedDiscussionsCountAsync(int Id);
        public Task<bool> IsShortLinkFreeAsync(int Id, string? Shortlink);
        public Task<bool> IsThisDiscussionMutedAsync(int Id, int UserId);
    }
}
