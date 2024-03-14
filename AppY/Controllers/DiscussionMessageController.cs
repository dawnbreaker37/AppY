using AppY.Abstractions;
using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace AppY.Controllers
{
    public class DiscussionMessageController : Controller
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;
        private readonly Message _messages;

        public DiscussionMessageController(Context context, Message messages, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
            _messages = messages;
        }

        [HttpGet]
        public async Task<IActionResult> LoadMessages(int Id, int SkipCount)
        {
            int UserId = GetCurrentUserIdFromCookies();
            if(UserId != 0)
            {
                IQueryable<DiscussionMessage>? Result_Preview = _messages.GetMessages(Id, UserId, SkipCount, 35);
                if (Result_Preview != null)
                {
                    List<DiscussionMessage>? Result = await Result_Preview.ToListAsync();
                    if (Result != null) return Json(new { success = true, result = Result, count = Result.Count });
                }
            }
            return Json(new { success = false, alert = "No more messages to load" });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessage Model)
        {
            if(ModelState.IsValid && User.Identity.IsAuthenticated)
            {              
                int Result = await _messages.SendMessageAsync(Model);
                if (Result != 0) return Json(new { success = true, result = Model });
            }
            return Json(new { success = false, alert = "An error unexpected so it's unable to send a message" });
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageInfo(int Id, int UserId)
        {
            DiscussionMessage? Result = await _messages.GetMessageInfoAsync(Id, UserId);
            if (Result != null)
            {
                Result.DaysPassed = DateTime.Now.Subtract(Result.SentAt).Days;
                return Json(new { success = true, result = Result, id = Id, userId = UserId });
            }
            else return Json(new { success = false, alert = "Sorry, but we're unable to get any information about that message" });
        }

        [HttpPost]
        public async Task<IActionResult> EditMessage(SendMessage Model)
        {
            if (ModelState.IsValid)
            {
                int UserId = GetCurrentUserIdFromCookies();
                if(UserId != 0)
                {
                    Model.UserId = UserId;
                    int Result = await _messages.EditMessageAsync(Model);
                    if (Result != 0) return Json(new { success = true, id = Result, text = Model.Text });
                }
            }
            return Json(new { success = false, alert = "Unable to edit this message. Please, check all datas and then try to edit it again" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id, int DiscussionId)
        {
            int UserId = GetCurrentUserIdFromCookies();
            if(UserId != 0)
            {
                int Result = await _messages.DeleteMessageAsync(Id, UserId, DiscussionId);
                if (Result != 0) return Json(new { success = true, id = Result });
            }
            return Json(new { success = false, alert = "We're sorry, but we cannot delete that message now, because something has gone wrong. Please, try to delete this message later" });
        }

        public int GetCurrentUserIdFromCookies()
        {
            string? CurrentUserId_Str = null;
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (CurrentUserId_Str != null)
                {
                    bool TryToParseUserId = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryToParseUserId) return UserId;
                }
            }
            else
            {
                CurrentUserId_Str = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (CurrentUserId_Str != null)
                {
                    bool TryToParseUserId = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryToParseUserId)
                    {
                        Response.Cookies.Append("CurrentUserId", CurrentUserId_Str);
                        return UserId;
                    }
                }
            }
            return 0;
        }
    }
}
