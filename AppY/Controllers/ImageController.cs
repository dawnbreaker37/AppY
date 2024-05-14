using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppY.Controllers
{
    public class ImageController : Controller
    {
        private readonly Context _context;
        private readonly Image _image;
        private readonly SavedMessageImage _savedImage;

        public ImageController(Context context, Image image, SavedMessageImage savedImage)
        {
            _context = context;
            _image = image;
            _savedImage = savedImage;
        }

        [HttpGet]
        public async Task<IActionResult> GetSMCount(int Id)
        {
            int Result = await _savedImage.GetMessageImagesCountAsync(Id);
            return Json(new { success = true, result = Result });
        }

        [HttpGet]
        public async Task<IActionResult> GetSMNext(int Id, int SkipCount, int FullCount, bool StartTry)
        {
            if (FullCount <= 0) FullCount = await _savedImage.GetMessageImagesCountAsync(Id);
            SkipCount = SkipCount >= FullCount ? 1 : ++SkipCount;
            SavedMessageContentImage? Result = await _savedImage.GetNextImageAsync(Id, SkipCount, FullCount, StartTry);

            if(Result != null) return Json(new { success = true, result = Result, fullCount = FullCount, skipCount = SkipCount });
            else return Json(new { success = false, alert = "Unable to get any info about this image" });
        }

        [HttpGet]
        public async Task<IActionResult> GetSMPrev(int Id, int SkipCount, int FullCount)
        {
            SkipCount = SkipCount > 0 ? --SkipCount : --FullCount;

            SavedMessageContentImage? Result = await _savedImage.GetPrevImageAsync(Id, SkipCount);
            if (Result != null) return Json(new { success = true, result = Result, fullCount = FullCount, skipCount = SkipCount });
            return Json(new { success = false, alert = "Unable to get any info about this image" });
        }

        [HttpGet]
        public async Task<IActionResult> GetNext(int Id, int SkipCount, int FullCount, bool StartTry)
        {
            DiscussionMessageImage? Result = await _image.GetNextImageAsync(Id, SkipCount, FullCount, StartTry);
            if (Result != null)
            {
                if (FullCount <= 0) FullCount = await _image.GetMessageImagesCountAsync(Id);
                return Json(new { success = true, result = Result, startTry = StartTry, skipCount = SkipCount >= FullCount ? 0 : ++SkipCount, fullCount = FullCount });
            }
            else return Json(new { success = false, alert = "No images to load" });
        }

        [HttpGet]
        public async Task<IActionResult> GetPrev(int Id, int SkipCount, int FullCount)
        {
            DiscussionMessageImage? Result = await _image.GetPrevImageAsync(Id, SkipCount);
            if (Result != null)
            {
                if (FullCount <= 0) FullCount = await _image.GetMessageImagesCountAsync(Id);
                return Json(new { success = true, result = Result, skipCount = SkipCount > 0 ? --SkipCount : 0, fullCount = FullCount });
            }
            else return Json(new { success = false, alert = "No images to load" });
        }
    }
}
