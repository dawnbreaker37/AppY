using AppY.Abstractions;
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
        private readonly Message _messages;

        public DiscussionController(Context context, Message messages, IDiscussion discussion, IUser user)
        {
            _context = context;
            _discussion = discussion;
            _messages = messages;
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
            IQueryable<DiscussionShortInfo>? Discussion_Preview = _discussion.GetUserDiscussions(Id);
            if(Discussion_Preview != null)
            {
                List<DiscussionShortInfo>? Discussions = await Discussion_Preview.ToListAsync();
                if (Discussions != null) return Json(new { success = true, result = Discussions, count = Discussions.Count });
            }
            return Json(new { success = false, count = 0 });
        }

        [HttpGet]
        public async Task<IActionResult> GetDeletedDiscussions(int Id)
        {
            IQueryable<DiscussionShortInfo>? DeletedDiscussions_Preview = _discussion.GetUserDeletedDiscussions(Id);
            if(DeletedDiscussions_Preview != null)
            {
                List<DiscussionShortInfo>? DeletedDiscussions = await DeletedDiscussions_Preview.ToListAsync();
                if (DeletedDiscussions != null) return Json(new { success = true, result = DeletedDiscussions, count = DeletedDiscussions.Count });
            }
            return Json(new { success = false, alert = "You haven't got any deleted discussion for now" });
        }

        [HttpGet]
        public async Task<IActionResult> IsShortlinkFree(int Id, string? Shortlink)
        {
            bool Result = await _discussion.IsShortLinkFreeAsync(Id, Shortlink);
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
                List<DiscussionMessage>? Messages = null;
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
                            User? CreatorInfo;
                            bool IsThisDiscussionMuted = await _discussion.IsThisDiscussionMutedAsync(Id, UserId);
                            //bool IsThisDiscussionMuted = UserInfo.MutedDiscussions != null ? UserInfo.MutedDiscussions.Any(d => d.DiscussionId == Id) : false;
                            int MessagesCount = await _messages.SentMessagesCountAsync(Id);
                            if (MessagesCount > 0)
                            {
                                IQueryable<DiscussionMessage>? Result_Preview = _messages.GetMessages(Id, UserId, 0, 35);
                                if (Result_Preview != null) Messages = await Result_Preview.ToListAsync();
                            }

                            if (DiscussionInfo.CreatorId != UserInfo.Id) CreatorInfo = await _user.GetUserSuperShortInfoAsync(DiscussionInfo.CreatorId);
                            else CreatorInfo = new User { PseudoName = UserInfo.PseudoName, ShortName = UserInfo.ShortName };

                            await _messages.MarkAsReadAllMessagesAsync(Id, UserId);

                            ViewBag.UserInfo = UserInfo;
                            ViewBag.CreatorInfo = CreatorInfo;
                            ViewBag.Discussion = DiscussionInfo;
                            ViewBag.MembersCount = await _discussion.GetMembersCountAsync(Id);
                            ViewBag.MessagesCount = MessagesCount;
                            ViewBag.Messages = Messages;
                            ViewBag.IsThisDiscussionMuted = IsThisDiscussionMuted;
                            ViewBag.DiscussionAvatar = DiscussionInfo.AvatarUrl == null ? DiscussionInfo.Name?[0].ToString() : DiscussionInfo.AvatarUrl;

                            return View();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Create", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersInfo(int Id)
        {
            IQueryable<DiscussionUsers>? MembersInfo_Preview = _discussion.GetMembersInfo(Id);
            if (MembersInfo_Preview != null)
            {
                List<DiscussionUsers>? Result = await MembersInfo_Preview.ToListAsync();
                if (Result != null) return Json(new { success = true, result = Result, count = Result.Count });
                else return Json(new { success = true, count = 0 });
            }
            else return Json(new { success = false, alert = "Unable to get any information about the members of this discussion. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int Id, int AdderId, int UserId)
        {
            Console.WriteLine(Id + ", " + AdderId + ", " + UserId);
            int Result = await _discussion.AddMemberAsync(Id, AdderId, UserId);
            if (Result > 0) return Json(new { success = true, id = Result, alert = "User added to discussion" });
            else if (Result == 0) return Json(new { success = false, error = Result, alert = "An unexpected error occured. Please, try again later" });
            else if (Result == -128) return Json(new { success = false, error = Result, alert = "You can't add yourself to a discussion" });
            else return Json(new { success = false, error = Result, alert = "This user already is in this discussion" });
        }

        [HttpPost]
        public async Task<IActionResult> Mute(int Id, int UserId)
        {
            int Result = await _discussion.MuteAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, alert = "Notifications from this discussion are muted" });
            else return Json(new { success = false, alert = "Sorry, but an unexpected error occured. Please, try to mute this discussion later" });
        }

        [HttpPost]
        public async Task<IActionResult> Unmute(int Id, int UserId)
        {
            int Result = await _discussion.UnmuteAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, alert = "Notifications from this discussion are unmuted" });
            else return Json(new { success = false, alert = "Sorry, but an unexpected error occured. Please, try to unmute this discussion later" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id, int UserId, string? DiscussionName)
        {
            int Result = await _discussion.DeleteDiscussionAsync(Id, UserId, DiscussionName);
            if (Result > 0) return Json(new { success = true, alert = "This discussion has been successfully deleted. All the users except you has received a notification about it" });
            else return Json(new { success = false, alert = "This discussion cannot be deleted by your hands" });
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int Id, int UserId)
        {
            int Result = await _discussion.RestoreDiscussionAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, id = Id, alert = "You've successfully restored the discussion" });
            else return Json(new { success = false, alert = "We're sorry, but you cannot restore this discussion" });
        }
    }
}
