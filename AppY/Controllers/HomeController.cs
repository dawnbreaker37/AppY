using AppY.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace AppY.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                string? CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(CurrentUserId != null)
                {
                    HttpContext.Response.Cookies.Append("CurrentUserId", CurrentUserId);
                }
            }
            return View();
        }

        public IActionResult Toolbox()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
