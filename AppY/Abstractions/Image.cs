using AppY.Models;

namespace AppY.Abstractions
{
    public abstract class Image
    {
        public abstract Task<int> GetMessageImagesCountAsync(int Id);
        public abstract Task<DiscussionMessageImage?> GetNextImageAsync(int Id, int SkipCount, int FullCount, bool StartTry);
        public abstract Task<DiscussionMessageImage?> GetPrevImageAsync(int Id, int SkipCount);
    }
}
