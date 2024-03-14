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

        public UserController(Context context, UserManager<User> userManager, IUser user, INotification notification, IDiscussion discussion, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _user = user;
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
    }
}
