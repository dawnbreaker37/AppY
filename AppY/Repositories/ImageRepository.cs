using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class ImageRepository : Image
    {
        private readonly Context _context;

        public ImageRepository(Context context)
        {
            _context = context;
        }

        public async override Task<DiscussionMessageImage?> GetNextImageAsync(int Id, int SkipCount, int FullCount, bool StartTry)
        {
            if (Id > 0)
            {
                if (StartTry) SkipCount = 0;
                else SkipCount = SkipCount >= FullCount ? 0 : ++SkipCount;
                return await _context.DiscussionMessageImages.AsNoTracking().Where(d => d.MessageId == Id).Select(d => new DiscussionMessageImage { Id = d.Id, Url = d.Url }).Skip(SkipCount).FirstOrDefaultAsync();
            }
            else return null;
        }

        public async override Task<DiscussionMessageImage?> GetPrevImageAsync(int Id, int SkipCount)
        {
            if (Id > 0)
            {
                SkipCount = SkipCount > 0 ? --SkipCount : 0;
                return await _context.DiscussionMessageImages.AsNoTracking().Where(d => d.MessageId == Id).Select(d => new DiscussionMessageImage { Id = d.Id, Url = d.Url }).Skip(SkipCount).FirstOrDefaultAsync();
            }
            else return null;
        }

        public async override Task<int> GetMessageImagesCountAsync(int Id)
        {
            if (Id > 0) return await _context.DiscussionMessageImages.AsNoTracking().CountAsync(d => d.MessageId == Id);
            else return 0;
        }
    }
}
