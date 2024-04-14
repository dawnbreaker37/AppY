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

        public ImageController(Context context, Image image)
        {
            _context = context;
            _image = image;
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
