using AppY.Controllers;
using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IDiscussion
    {
        public Task<int> CreateDiscussionAsync(Discussion_ViewModel Model);
        public Task<bool> EditDiscussionAsync(Discussion_ViewModel Model);
        public IQueryable<DiscussionShortInfo>? GetUserDiscussion(int Id);
        public Task<Discussion?> GetDiscussionInfoAsync(int Id);
        public Task<int> GetMembersCountAsync(int Id);
        public Task<bool> IsShortLinkFree(int Id, string? Shortlink);
    }
}
