using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text;

namespace AppY.Controllers
{
    public class UserController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IUser _user;
        private readonly INotification _notification;
        private readonly IDiscussion _discussion;
        private readonly IMemoryCache _memoryCache;
        private readonly IChat _chat;

        public UserController(Context context, UserManager<User> userManager, IChat chat, IUser user, INotification notification, IDiscussion discussion, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _user = user;
            _chat = chat;
            _discussion = discussion;
            _notification = notification;
            _memoryCache = memoryCache;
        }
        
        public async Task<IActionResult> Account()
        {
            if (User.Identity.IsAuthenticated)
            {                
                string? CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool ParseResult = Int32.TryParse(CurrentUserId, out int UserId);
                if (ParseResult)
                {
                    User? UserInfo = await _user.GetMainUserInfoAsync(UserId);
                    if (UserInfo != null)
                    {
                        if (Request.Cookies.ContainsKey("CurrentUserId"))
                        {
                            HttpContext.Response.Cookies.Delete("CurrentUserId");
                            HttpContext.Response.Cookies.Append("CurrentUserId", UserInfo.Id.ToString());
                        }
                        else HttpContext.Response.Cookies.Append("CurrentUserId", UserInfo.Id.ToString());

                        string? DaysPassedString = null;
                        bool IsPasswordChangeAllowed = false;
                        TimeSpan? DaysPassed;
                        if (UserInfo.PasswordChanged is not null) DaysPassed = DateTime.Now.Subtract((DateTime)UserInfo.PasswordChanged);
                        else DaysPassed = null;

                        if ((DaysPassed is null) || (DaysPassed.HasValue && DaysPassed.Value.Days >= 28)) IsPasswordChangeAllowed = true;
                        else
                        {
                            DaysPassedString = DaysPassed.Value.Days > 0 ? DaysPassed.Value.Days + " days, " : null;
                            DaysPassedString += DaysPassed.Value.Hours > 0 ? DaysPassed.Value.Hours + " hr." : DaysPassed.Value.Minutes + " min.";
                        }

                        int DeletedDiscussionsCount = await _discussion.GetDeletedDiscussionsCountAsync(UserId);

                        ViewBag.UnpicturedAvatarInfo = _user.UnpicturedAvatarSelector(UserInfo);
                        ViewBag.UserInfo = UserInfo;
                        ViewBag.DeletedDiscussionsCount = DeletedDiscussionsCount;
                        ViewBag.DaysPassed = DaysPassed;
                        ViewBag.DaysPassedStr = DaysPassedString;
                        ViewBag.IsPasswordChangeAllowed = IsPasswordChangeAllowed;

                        return View();
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Create", "Account");
        }

        public async Task<IActionResult> Page(string? Id)
        {
            if(User.Identity.IsAuthenticated)
            {
                User? UserInfo;
                bool TryToParse = Int32.TryParse(Id, out int IntegerUserId);
                if (TryToParse) UserInfo = await _user.GetAverageUserInfoAsync(IntegerUserId);
                else UserInfo = await _user.GetAverageUserInfoAsync(Id);

                if(UserInfo != null)
                {
                    int CurrentUserId = 0;
                    if (Request.Cookies.ContainsKey("CurrentUserId"))
                    {
                        string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                        if (!String.IsNullOrWhiteSpace(CurrentUserId_Str)) Int32.TryParse(CurrentUserId_Str, out CurrentUserId);
                    }

                    if (UserInfo.Id != CurrentUserId)
                    {
                        UserInfo.LastSeenText = _user.SetLastSeenText(UserInfo.LastSeen);
                        int DiscussionsCount = await _discussion.GetDiscussionsCountAsync(UserInfo.Id);
                        if (UserInfo.AvatarUrl == null) UserInfo.UnpicturedAvatarInfo = UserInfo.AvatarStickerUrl == null ? UserInfo.PseudoName?[0].ToString() : UserInfo.AvatarStickerUrl;
                        int ChatId = await _chat.FindChatAvailability(UserInfo.Id, CurrentUserId, true);

                        ViewBag.UserInfo = UserInfo;
                        ViewBag.CurrentUserId = CurrentUserId;
                        ViewBag.ChatId = ChatId;
                        ViewBag.DiscussionsCount = await _discussion.GetDiscussionsCountAsync(UserInfo.Id);
                        ViewBag.DiscussionsCount = DiscussionsCount;

                        return View();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("Create", "Account");
        }

        public async Task<IActionResult> Notifications()
        {
            if(User.Identity.IsAuthenticated)
            {
                string? UserId_Str = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (UserId_Str != null)
                {
                    bool TryParseUserId = Int32.TryParse(UserId_Str, out int UserId);
                    if (TryParseUserId)
                    {
                        User? UserInfo = await _user.GetMainUserInfoAsync(UserId);
                        await _notification.DeleteAncientNotificationsAsync(UserId);
                        IQueryable<NotificationModel>? NotificationsPreview = _notification.GetNotifications(UserId, 0, 35);
                        if (NotificationsPreview != null)
                        {
                            List<NotificationModel>? Notifications = await NotificationsPreview.ToListAsync();
                            if (Notifications != null)
                            {
                                ViewBag.UserInfo = UserInfo;
                                ViewBag.Notifications = Notifications;
                                ViewBag.MissedNotificationsCount = Notifications.Count(n => !n.IsChecked);
                                ViewBag.CheckedNotificationsCount = Notifications.Count(n => n.IsChecked);
                                ViewBag.NotificationsCount = Notifications.Count;

                                return View();
                            }
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("Create", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserInfo_ViewModel Model)
        {
            if(ModelState.IsValid)
            {
                string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool TryParseUserId = Int32.TryParse(UserId, out int ModelId);
                if (TryParseUserId) Model.Id = ModelId;

                bool Result = await _user.EditUserInfoAsync(Model);
                if (Result) return Json(new { success = true, result = Model, alert = "Edit completed successfully" });             
            }
            return Json(new { success = false, alert = "Unable to edit. Please, check all datas and try again" });
        }

        [HttpPost]
        public async Task<IActionResult> EditBatterySettings(int Id, int EcoModeOnAt)
        {
            int Result = await _user.EditEcoModeSettings(Id, EcoModeOnAt);
            if (Result > 0) return Json(new { success = true, alert = "Battery level settings has been edited", result = Result });
            else return Json(new { success = false, alert = "Sorry, but an unexpected error has occured. Please, try to edit battery settings later" });
        }

        [HttpPost]
        public async Task<IActionResult> EditAvatarDesign(EditAvatarColors Model)
        {
            if (ModelState.IsValid)
            {
                bool Result = await _user.EditAvatarDesignAsync(Model);
                if (Result) return Json(new { success = true, alert = "Avatar design has been successfully updated", bgColor = Model.BgColor, fgColor = Model.FgColor, sticker = Model.AvatarStickerUrl });
            }
            return Json(new { success = false, alert = "An unexpected error occured. Please, check all datas and try to edit avatar design again" });
        }

        [HttpPost]
        public async Task<IActionResult> EditPrivacy(EditUserPrivacySettings Model)
        {
            if(ModelState.IsValid)
            {
                bool Result = await _user.EditPrivacySettingsAsync(Model);
                if (Result) return Json(new { success = true, alert = "Privacy settings has been edited" });
            }
            return Json(new { success = false, alert = "Unable to edit privacy settings" });
        }

        [HttpPost]
        public async Task<IActionResult> SetProfilePhoto(int Id, IFormFile File)
        {
            if (ModelState.IsValid)
            {
                string? Result = await _user.SetProfilePhotoAsync(Id, File);
                if (!String.IsNullOrEmpty(Result)) return Json(new { success = true, result = Result });
            }
            return Json(new { success = false, alert = "Wrong image format. The only accepted extension are: <span class='fw-500'>.jpg</span>, <span class='fw-500'>.jpeg</span> and <span class='fw-500'>.png</span>" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfilePhoto(int Id)
        {
            bool Result = await _user.DeleteProfilePhotoAsync(Id, null);
            if (Result) return Json(new { success = true });
            else return Json(new { success = false, alert = "We're sorry, but an unexpected error occured. Please, try to delete your profile photo later" });
        }

        [HttpGet]
        public async Task<IActionResult> IsShortnameUnique(int Id, string? Shortname)
        {
            bool Result = await _user.IsShortnameUniqueAsync(Id, Shortname);
            if (Result) return Json(new { success = true, result = Shortname });
            else return Json(new { success = false, });
        }

        [HttpGet]
        public async Task<IActionResult> FindToAdd(string Keyword)
        {
            IQueryable<User>? Results_Preview = _user.FindUsers(Keyword);
            if(Results_Preview != null)
            {
                List<User>? Results = await Results_Preview.ToListAsync();
                if (Results != null) return Json(new { success = true, result = Results, count = Results.Count });
            }
            return Json(new { success = false, alert = "No user found. Try other keywords to find new members for this discussion" });
        }

        [HttpGet]
        public async Task<IActionResult> FindByShortname(string? Shortname)
        {
            User? UserSuperShortInfo = await _user.GetUserSuperShortInfoAsync(Shortname);
            if (UserSuperShortInfo != null)
            {
                return Json(new { success = true, result = UserSuperShortInfo });
            }
            else return Json(new { success = false, alert = "We haven't found any user with that kind of shortname" });
        }

        [HttpGet]
        public async Task<IActionResult> FindById(int Id)
        {
            User? UserSuperShortInfo = await _user.GetUserSuperShortInfoAsync(Id);
            if (UserSuperShortInfo != null) return Json(new { success = true, result = UserSuperShortInfo });
            else return Json(new { success = false, alert = "Unfortunately, we haven't found any user with that Identifier, or selected user has private account so we can't get any additional info about him" });
        }

        [HttpGet]
        public async Task<IActionResult> SubmitReserveCode(int Id, string? ReserveCode)
        {
            bool Result = await _user.SubmitReserveCodeAsync(Id, ReserveCode);
            if (Result) return Json(new { success = true, alert = "Reserve code submitted", id = Id });
            else return Json(new { success = false, alert = "Entered reserve code was wrong" });
        }

        [HttpPost]
        public async Task<IActionResult> SetLastSeen()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    if (CurrentUserId_Str != null)
                    {
                        bool TryParseResult = Int32.TryParse(CurrentUserId_Str, out int CurrentUserId);
                        if(TryParseResult)
                        {
                            int Result = await _user.SetLastSeenAsync(CurrentUserId);
                            if (Result > 0) return Json(new { success = true });
                        }
                    }
                }
            }
            return Json(new { success = false });
        }
    }
}
