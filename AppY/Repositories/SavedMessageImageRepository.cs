using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class SavedMessageImageRepository : SavedMessageImage
    {
        private readonly Context _context;
        
        public SavedMessageImageRepository(Context context)
        {
            _context = context;
        }

        public override async Task<int> GetMessageImagesCountAsync(int Id)
        {
            return await _context.SavedMessagesContentImages.AsNoTracking().CountAsync(s => s.SavedMessageId == Id && !s.IsDeleted);
        }

        public override async Task<SavedMessageContentImage?> GetNextImageAsync(int Id, int SkipCount, int FullCount, bool StartTry)
        {
            SkipCount = SkipCount >= FullCount ? 0 : SkipCount;
            return await _context.SavedMessagesContentImages.AsNoTracking().Where(s => s.SavedMessageId == Id && !s.IsDeleted).Select(s => new SavedMessageContentImage {Id = s.Id, Name = s.Name, SkipCount = SkipCount }).Skip(SkipCount).FirstOrDefaultAsync();
        }

        public async override Task<SavedMessageContentImage?> GetPrevImageAsync(int Id, int SkipCount)
        {
            SkipCount = SkipCount <= 0 ? 0 : SkipCount;
            return await _context.SavedMessagesContentImages.AsNoTracking().Where(s => s.SavedMessageId == Id && !s.IsDeleted).Select(s => new SavedMessageContentImage { Id = s.Id, Name = s.Name, SkipCount = SkipCount }).Skip(SkipCount).FirstOrDefaultAsync();
        }
    }
}
