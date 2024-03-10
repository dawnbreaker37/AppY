using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppY.Controllers
{
    public class DiscussionController : Controller
    {
        private readonly Context _context;
        private readonly IDiscussion _discussion;
        private readonly IUser _user;
        private readonly IDiscussionMessage _discussionMessage;

        public DiscussionController(Context context, IDiscussion discussion, IDiscussionMessage discussionMessage, IUser user)
        {
            _context = context;
            _discussion = discussion;
            _discussionMessage = discussionMessage;
            _user = user;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Discussion_ViewModel Model)
        {
            if(ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                string? UserId_Str = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool Result = Int32.TryParse(UserId_Str, out int UserId);
                if(Result)
                {
                    Model.UserId = UserId;
                    int DiscussionResult = await _discussion.CreateDiscussionAsync(Model);
                    if (DiscussionResult != 0) if (Model.IsPrivate) return Json(new { success = true, isPrivate = true, password = Model.Password, result = DiscussionResult });
                    else return Json(new { success = true, isPrivate = false, result = DiscussionResult });
                }
                else return Json(new { success = false, alert = "Unable to create discussion. Please, check all datas and try again" });
            }
            return Json(new { success = false, alert = "Sign in or log in to create a discussion" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscussions(int Id)
        {
            IQueryable<DiscussionShortInfo>? Discussion_Preview = _discussion.GetUserDiscussion(Id);
            if(Discussion_Preview != null)
            {
                List<DiscussionShortInfo>? Discussions = await Discussion_Preview.ToListAsync();
                if (Discussions != null) return Json(new { success = true, result = Discussions, count = Discussions.Count });
            }
            return Json(new { success = false, count = 0 });
        }

        [HttpGet]
        public async Task<IActionResult> IsShortlinkFree(int Id, string? Shortlink)
        {
            bool Result = await _discussion.IsShortLinkFree(Id, Shortlink);
            if (Result) return Json(new { success = true, result = Result });
            else return Json(new { success = false, result = Result });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Discussion_ViewModel Model)
        {
            if (ModelState.IsValid)
            {
                bool Result = await _discussion.EditDiscussionAsync(Model);
                if (Result) return Json(new { success = true, result = Model });
                else return Json(new { success = false, alert = "Entered shortlink isn't unique. Please, choose another one or left your old one" });
            }
            else return Json(new { success = false, alert = "We're sorry, but something's wrong with your entered datas. Please, check them and then try to edit again" });
        }

        public async Task<IActionResult> Discuss(int Id)
        {
            if(Id != 0 && User.Identity.IsAuthenticated)
            {
                string? Str_UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool Result = Int32.TryParse(Str_UserId, out int UserId);
                if (Result)
                {
                    User? UserInfo = await _user.GetMainUserInfoAsync(UserId);
                    if (UserInfo != null)
                    {
                        Discussion? DiscussionInfo = await _discussion.GetDiscussionInfoAsync(Id);
                        if (DiscussionInfo != null)
                        {
                            ViewBag.UserInfo = UserInfo;
                            ViewBag.Discussion = DiscussionInfo;
                            ViewBag.MembersCount = await _discussion.GetMembersCountAsync(Id);
                            ViewBag.MessagesCount = await _discussionMessage.SentMessagesCountAsync(Id);
                            ViewBag.DiscussionAvatar = DiscussionInfo.AvatarUrl == null ? DiscussionInfo.Name?[0].ToString() : DiscussionInfo.AvatarUrl;

                            return View();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Create", "Account");
        }
    }
}
