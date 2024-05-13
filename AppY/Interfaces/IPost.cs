using AppY.Models;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IPost
    {
        public Task<int> GetUserPostsCountAsync(int Id);
        public IQueryable<Post>? GetUserPosts(int Id, int SkipCount);
        public Task<bool> AddPostAsync(Post_ViewModel Model);
        public Task<int> EditPostAsync(Post_ViewModel Model);
        public Task<int> DeletePostAsync(int Id, int UserId);
    }
}
