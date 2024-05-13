using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppY.Controllers
{
    public class PostController : Controller
    {
        private readonly Context _context;
        private readonly IPost _post;
        private readonly IUser _user;

        public PostController(Context context, IPost post, IUser user)
        {
            _context = context;
            _post = post;
            _user = user;
        }

        public async Task<IActionResult> AddPost()
        {
            if(User.Identity.IsAuthenticated)
            {
                string? UserId_Str = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool TryParse = Int32.TryParse(UserId_Str, out int UserId);
                if(TryParse)
                {
                    User? UserInfo = await _user.GetMainUserInfoAsync(UserId);
                    if(UserInfo != null)
                    {
                        ViewBag.UserInfo = UserInfo;

                        return View();
                    }
                }
            }
            return RedirectToAction("Create", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Add(Post_ViewModel Model)
        {
            if(ModelState.IsValid)
            {
                if(Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId = Request.Cookies["CurrentUserId"];
                    bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                    if(TryParse)
                    {
                        Model.UserId = UserId;
                        bool Result = await _post.AddPostAsync(Model);
                        if (Result) return Json(new { success = true });
                        else return Json(new { success = false, alert = "Something is wrong. Please, check all your datas and then try to add your post again" });
                    }
                }
            }
            return Json(new { success = false, alert = "You can't add post. For additional info please check QA page" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserPosts(int SkipCount)
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if(TryParse)
                {
                    IQueryable<Post>? Result_Preview = _post.GetUserPosts(UserId, SkipCount);
                    if(Result_Preview != null)
                    {
                        List<Post>? Result = await Result_Preview.ToListAsync();
                        if (Result != null) return Json(new { success = true, result = Result, loadedCount = Result.Count, totalCount = Result.Count + SkipCount });
                    }
                    return Json(new { success = false, alert = "No more posts to load", totalCount = SkipCount });
                }
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error has occured. Please, try to get your posts a bit later", totalCount = SkipCount });
        }
    }
}
