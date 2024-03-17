using AppY.Interfaces;
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
        private readonly IUser _user;

        public HomeController(ILogger<HomeController> logger, IUser user)
        {
            _logger = logger;
            _user = user;
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

        public async Task<IActionResult> Search()
        {
            string? UserId_Str = null;
            User? UserInfo = null;
            if (User.Identity.IsAuthenticated)
            {
                if(Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    UserId_Str = Request.Cookies["CurrentUserId"];
                    bool TryParse = Int32.TryParse(UserId_Str, out int UserId);
                    if(TryParse)
                    {
                        UserInfo = await _user.GetMainUserInfoAsync(UserId);
                    }
                }
            }
            ViewBag.UserInfo = UserInfo;

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
