using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppY.Controllers
{
    public class NotificationController : Controller
    {
        private readonly Context _context;
        private readonly INotification _notifcation;

        public NotificationController(Context context, INotification notifcation)
        {
            _context = context;
            _notifcation = notifcation;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(Notifications_ViewModel Model)
        {
            if (ModelState.IsValid)
            {
                int Result = await _notifcation.SendNotificationAsync(Model);
                if (Result != 0) return Json(new { success = true, alert = "Notification has been successfully sent", id = Result });
                else return Json(new { success = false, alert = "Notification failure. Unable to send it now, please, try again later" });
            }
            else return Json(new { success = false, alert = "Please, check all your entered datas and then try to send your notification again" });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationInfo(int Id, int UserId)
        {
            NotificationModel? NotificationInfo = await _notifcation.GetNotificationInfoAsync(Id, UserId);
            if (NotificationInfo != null) return Json(new { success = true, result = NotificationInfo });
            else return Json(new { success = false, alert = "We're sorry, but an unexpected error occured. Please, try to get this notification additional information later" });
        }

        [HttpPost]
        public async Task<IActionResult> MarkasRead(int Id, int UserId)
        {
            int Result = await _notifcation.MarkAsReadAsync(Id, UserId);
            if (Result != 0) return Json(new { success = true, id = Result });
            else return Json(new { success = false, alert = "False prompt. Please, try to mark it again later" });
        }

        [HttpPost]
        public async Task<IActionResult> Pin(int Id, int UserId)
        {
            int Result = await _notifcation.PinAsync(Id, UserId);
            if (Result != 0) return Json(new { success = true, alert = "Notification has been successfully pinned", id = Result });
            else return Json(new { success = false, alert = "We're sorry, but we cannot pin this notification" });
        }

        [HttpPost]
        public async Task<IActionResult> Unpin(int Id, int UserId)
        {
            int Result = await _notifcation.UnpinAsync(Id, UserId);
            if (Result != 0) return Json(new { success = true, alert = "Notification has been successfully unpinned. Notice that it'll be automatically deleted after few days if that notification is not a pinned or untouchable notification", id = Result });
            else return Json(new { success = false, alert = "We're sorry, but we cannot pin this notification" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id, int UserId)
        {
            int Result = await _notifcation.DeleteNotificationAsync(Id, UserId);
            if (Result != 0) return Json(new { success = true, id = Id });
            else return Json(new { success = false, alert = "We're sorry, but it seems that this notification is <span class='fw-500'>untouchable</span> or <span class='fw-500'>pinned</span>, so it cannot be removed. Please, unpin it if it's a pinned notification or wait until it automatically deletes from your list if it's an untouchable notification. Read more in <span class='fw-500'>About</span> tab of your notification widget" });
        }
    }
}
