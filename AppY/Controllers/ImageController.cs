using Microsoft.AspNetCore.Mvc;

namespace AppY.Controllers
{
    public class ImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
