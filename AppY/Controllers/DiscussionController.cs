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

        [HttpPost]
        public async Task<IActionResult> SetAvatar(int Id, int UserId, IFormFile Image)
        {
            string? Result = await _discussion.SetDiscussionAvatarAsync(Id, UserId, Image);
            if (Result is not null) return Json(new { success = true, result = Result });
            else return Json(new { success = false, alert = "We're sorry, but something went wrong. Please, try to set an avatar for this discussion later" });
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int Id, int UserId, string? Status)
        {
            string? Result = await _discussion.SetDiscussionStatusAsync(Id, UserId, Status);
            if (Result is not null) return Json(new { success = true, result = Result });
            else return Json(new { success = false, alert = "No access to edit the status" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvatar(int Id, int UserId)
        {
            bool Result = await _discussion.DeleteDiscussionAvatarAsync(Id, UserId);
            if (Result) return Json(new { success = true });
            else return Json(new { success = false, alert = "We're sorry, but something had gone wrong. Please, try to delete discussion's avatar later" });
        }

        [HttpGet]
        public async Task<IActionResult> GetShortInfo(int Id, int UserId)
        {
            Discussion? Info = await _discussion.GetDiscussionShortInfoAsync(Id, UserId);
            if (Info != null) return Json(new { success = true, result = Info });
            else return Json(new { success = false, alert = "We're sorry, but we haven't found any information about discussion" });
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

        public async Task<IActionResult> DiscussionInfo(string Id)
        {
            int ResultId = await _discussion.GetDiscussionIdByShortlinkAsync(Id);
            if (ResultId > 0) return RedirectToAction("Discuss", "Discussion", ResultId);
            else return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Discuss(int Id)
        {
            if(Id != 0 && User.Identity.IsAuthenticated)
            {
                List<IGrouping<DateTime, DiscussionMessage>>? Messages = null;
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
                            bool IsThisDiscussionMuted = false;
                            bool AccessValue = await _discussion.HasThisUserAccessToThisDiscussionAsync(UserId, Id);
                            string? AutodeleteDelayValue = UserInfo.AreMessagesAutoDeletable > 0 ? _user.AutodeleteDelay(UserInfo.AreMessagesAutoDeletable) : "disabled";
                            if(AccessValue) IsThisDiscussionMuted = await _discussion.IsThisDiscussionMutedAsync(Id, UserId);
                            //bool IsThisDiscussionMuted = UserInfo.MutedDiscussions != null ? UserInfo.MutedDiscussions.Any(d => d.DiscussionId == Id) : false;
                            int MessagesCount = await _messages.SentMessagesCountAsync(Id);
                            if (MessagesCount > 0 && ((!DiscussionInfo.IsPrivate) || (DiscussionInfo.IsPrivate && AccessValue)))
                            {
                                IQueryable<IGrouping<DateTime, DiscussionMessage>>? Result_Preview = _messages.GetMessages(Id, UserId, 0, 100);
                                if (Result_Preview != null)
                                {
                                    Messages = await Result_Preview.ToListAsync();
                                    //foreach(var Item in Messages)
                                    //{
                                    //    foreach (DiscussionMessage discussionMessage in Item)
                                    //    {
                                    //        if (discussionMessage.RepliedMessageId != null && discussionMessage.RepliesMsgShortText == null) discussionMessage.RepliesMsgShortText = "Deleted message";
                                    //    }
                                    //}
                                }
                            }

                            if (DiscussionInfo.CreatorId != UserInfo.Id) CreatorInfo = await _user.GetUserSuperShortInfoAsync(DiscussionInfo.CreatorId);
                            else CreatorInfo = new User { PseudoName = UserInfo.PseudoName, ShortName = UserInfo.ShortName };
                            DiscussionInfo.Status = DiscussionInfo.Status == null ? "Tap to edit the status" : DiscussionInfo.Status;

                            await _messages.MarkAsReadAllMessagesAsync(Id, UserId);

                            ViewBag.UserInfo = UserInfo;
                            ViewBag.AccessValue = AccessValue;
                            ViewBag.AutodeleteDelayValue = AutodeleteDelayValue;
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
                if (Result != null)
                {
                    int CurrentUserAccessLevel = 0;
                    int CurrentUserId = 0;
                    if (Request.Cookies.ContainsKey("CurrentUserId"))
                    {
                        string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                        if (CurrentUserId_Str != null)
                        {
                            bool TryToParse = Int32.TryParse(CurrentUserId_Str, out CurrentUserId);
                            if(TryToParse) CurrentUserAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, CurrentUserId);
                        }
                    }
                    return Json(new { success = true, currentUserId = CurrentUserId, currentUserAccessLevel = CurrentUserAccessLevel, result = Result, count = Result.Count });
                }
                else return Json(new { success = true, count = 0 });
            }
            else return Json(new { success = false, alert = "Unable to get any information about the members of this discussion. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccessLevel(int Id, int ChangerId, int UserId, int AccessLevel)
        {
            int Result = await _discussion.ChangeAccessLevel(Id, ChangerId, UserId, AccessLevel);
            if (Result >= 0) return Json(new { success = true, result = Result, userId = UserId, changerId = ChangerId });
            else return Json(new { success = false, alert = "We're sorry, but you cannot change access level for this user now. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int Id, int AdderId, int UserId)
        {
            int Result = await _discussion.AddMemberAsync(Id, AdderId, UserId);
            if (Result > 0)
            {
                int AdderAccessLevel = await _user.GetCurrentUserAccessLevelAsync(Id, AdderId);
                return Json(new { success = true, id = Result, adderAccessLevel = AdderAccessLevel, alert = "User added to discussion" });
            }
            else if (Result == 0) return Json(new { success = false, error = Result, alert = "An unexpected error occured. Please, try again later" });
            else if (Result == -128) return Json(new { success = false, error = Result, alert = "You can't add yourself to a discussion" });
            else return Json(new { success = false, error = Result, alert = "This user's already in discussion" });
        }

        [HttpPost]
        public async Task<IActionResult> Join(int Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if(Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryToParse)
                    {
                        int Result = await _discussion.JoinAsync(Id, UserId);
                        if (Result > 0) return Json(new { success = true, alert = "You've been successfully joined to the discussion" });
                        else return Json(new { success = false, alert = "We're sorry, but you cannot join to this discussion" });
                    }
                }
            }
            return Json(new { success = false, alert = "Unable to join to this discussion. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> JoinToPrivate(int Id, string? Password)
        {
            if (User.Identity.IsAuthenticated)
            {
                if(Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? UserId_Str = Request.Cookies["CurrentUserId"];
                    if(!String.IsNullOrWhiteSpace(UserId_Str))
                    {
                        bool TryToParse = Int32.TryParse(UserId_Str, out int UserId);
                        if(TryToParse)
                        {
                            int Result = await _discussion.JoinToPrivateAsync(Id, UserId, Password);
                            if (Result > 0) return Json(new { success = true, alert = "You've joined successfully" });
                            else if (Result == -256) return Json(new { success = false, alert = "Sorry, but you're already in this discussion" });
                            else if (Result == -128) return Json(new { success = false, alert = "Wrong password for this discussion" });
                        }
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error has occured. Try again a bit later, please" });
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryToParse)
                    {
                        int Result = await _discussion.LeaveAsync(Id, UserId);
                        if (Result > 0) return Json(new { success = true, alert = "You've been successfully quitted this discussion" });
                        else return Json(new { success = false, alert = "We're sorry, but you cannot quit this discussion now. Please, try to quit it again a bit later" });
                    }
                }
            }
            return Json(new { success = false, alert = "Unable to quit you from this discussion. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(int Id, int UserId)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (!String.IsNullOrWhiteSpace(CurrentUserId_Str))
                {
                    bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int BlockerId);
                    if (TryToParse)
                    {
                        int Result = await _discussion.BlockUserAsync(Id, BlockerId, UserId);
                        if (Result > 0) return Json(new { success = true, alert = "User has been blocked successfully", result = UserId });
                        else return Json(new { success = false, alert = "You've no access to block that user" });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error occured so you cannot block that user now. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(int Id, int UserId)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (!String.IsNullOrWhiteSpace(CurrentUserId_Str))
                {
                    bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int BlockerId);
                    if (TryToParse)
                    {
                        int Result = await _discussion.UnblockUserAsync(Id, BlockerId, UserId);
                        if (Result > 0) return Json(new { success = true, alert = "User has been unblocked successfully", result = UserId });
                        else return Json(new { success = false, alert = "You've no access to unblock that user" });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error occured, so you cannot unblock that user now. Please, try to unblock later" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int Id, int UserId)
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if(!String.IsNullOrWhiteSpace(CurrentUserId_Str))
                {
                    bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int DeleterId);
                    if (TryToParse)
                    {
                        int Result = await _discussion.DeleteUserAsync(Id, DeleterId, UserId);
                        if (Result > 0) return Json(new { success = true, result = UserId });
                        else return Json(new { success = false, alert = "You've no access to delete that user" });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error has occured. Please, try this action again a bit later" });
        }

        [HttpPost]
        public async Task<IActionResult> Mute(int Id, int UserId)
        {
            int Result = await _discussion.MuteAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, result = Result, alert = "Notifications from this discussion are muted" });
            else return Json(new { success = false, alert = "Sorry, but an unexpected error occured. Please, try to mute this discussion later" });
        }

        [HttpPost]
        public async Task<IActionResult> Unmute(int Id, int UserId)
        {
            int Result = await _discussion.UnmuteAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, result = Result, alert = "Notifications from this discussion are unmuted" });
            else return Json(new { success = false, alert = "Sorry, but an unexpected error occured. Please, try to unmute this discussion later" });
        }

        [HttpPost]
        public async Task<IActionResult> Pin(int Id, int UserId)
        {
            int Result = await _discussion.PinAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, result = Result, alert = "Discussion has been pinned" });
            else return Json(new { success = false, alert = "Unable to pin that discussion" });
        }

        [HttpPost]
        public async Task<IActionResult> Unpin(int Id, int UserId)
        {
            int Result = await _discussion.UnpinAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, result = Result, alert = "Discussion has been unpinned" });
            else return Json(new { success = false, alert = "Unable to unpin that discussion. Please, try again later" });
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

        [HttpPost]
        public async Task<IActionResult> Find(string? Keyword)
        {
            IQueryable<DiscussionShortInfo>? Result_Preview = _discussion.Find(Keyword); 
            if(Result_Preview != null)
            {
                List<DiscussionShortInfo>? Result = await Result_Preview.ToListAsync();
                if (Result != null) return Json(new { success = true, result = Result, count = Result.Count });
            }
            return Json(new { success = false, count = 0, alert = "We're sorry, but we haven't found any discussion" });
        }
    }
}
