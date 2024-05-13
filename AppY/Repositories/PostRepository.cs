using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class PostRepository : Base<Post>, IPost
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostRepository(Context context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> AddPostAsync(Post_ViewModel Model)
        {
            if(Model.File != null)
            {
                string? FileExtension = Path.GetExtension(Model.File.Name);
                string? FileRandomName = Guid.NewGuid().ToString("N").Substring(2, 12);
                string? FileFullName = FileRandomName + FileExtension;
                using(FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "/PostFiles/" + FileFullName, FileMode.Create))
                {
                    await Model.File.CopyToAsync(fs);
                    Model.ImageUrl = FileFullName;
                }
            }

            Post post = new Post()
            {
                CanBeForwarded = Model.CanBeForwarded,
                AvailableTill = Model.AvailableTill > 0 ? DateTime.Now.AddMinutes(Model.AvailableTill) : null,
                CreatedAt = DateTime.Now,
                ImageUrl = Model.ImageUrl,
                IsDeleted = false,
                IsPinned = Model.IsPinned,
                Text = Model.Text,
                UserId = Model.UserId,
                Style = Model.Style
            };
            await _context.AddAsync(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> DeletePostAsync(int Id, int UserId)
        {
            DateTime? NullDate = null;
            int Result = await _context.Posts.AsNoTracking().Where(p => p.Id == Id && p.UserId == UserId).ExecuteUpdateAsync(p => p.SetProperty(p => p.IsDeleted, true).SetProperty(p => p.IsPinned, false).SetProperty(p => p.AvailableTill, NullDate));
            
            if (Result > 0) return Id;
            else return 0;
        }

        public Task<int> EditPostAsync(Post_ViewModel Model)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Post>? GetUserPosts(int Id, int SkipCount)
        {
            return _context.Posts.AsNoTracking().Where(p => p.UserId == Id && !p.IsDeleted).Select(p => new Post { Id = p.Id, IsPinned = p.IsPinned, CreatedAt = p.CreatedAt, Text = p.Text, ImageUrl = p.ImageUrl, CanBeForwarded = p.CanBeForwarded, AvailableTill = p.AvailableTill, Style = p.Style }).OrderByDescending(p => p.IsPinned).ThenByDescending(p => p.CreatedAt).Skip(SkipCount).Take(9);
        }

        public async Task<int> GetUserPostsCountAsync(int Id)
        {
            return await _context.Posts.AsNoTracking().CountAsync(p => p.UserId == Id && !p.IsDeleted);
        }
    }
}
