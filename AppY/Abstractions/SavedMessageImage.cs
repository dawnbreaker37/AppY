using AppY.Models;

namespace AppY.Abstractions
{
    public abstract class SavedMessageImage
    {
        public abstract Task<int> GetMessageImagesCountAsync(int Id);
        public abstract Task<SavedMessageContentImage?> GetNextImageAsync(int Id, int SkipCount, int FullCount, bool StartTry);
        public abstract Task<SavedMessageContentImage?> GetPrevImageAsync(int Id, int SkipCount);
    }
}
