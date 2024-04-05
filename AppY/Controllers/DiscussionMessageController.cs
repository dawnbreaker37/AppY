﻿using AppY.Abstractions;
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
                IQueryable<IGrouping<DateTime, DiscussionMessage>>? Result_Preview = _messages.GetMessages(Id, UserId, SkipCount, 35);
                if (Result_Preview != null)
                {
                    List<IGrouping<DateTime, DiscussionMessage>>? Result = await Result_Preview.ToListAsync();
                    if (Result != null) return Json(new { success = true, result = Result, count = Result.Count });
                }
            }
            return Json(new { success = false, alert = "No more messages to load" });
        }

        [HttpPost]
        public async Task<IActionResult> Message(SendMessage Model)
        {
            if(ModelState.IsValid && User.Identity.IsAuthenticated)
            {              
                string? Result = await _messages.SendMessageAsync(Model);
                if (Result != null && Model.Images != null) return Json(new { success = true, trueId = Model.Id, result = Model, imgUrl = Result, imgsCount = Model.Images.Count });
                else if (Result != null && Model.Images == null) return Json(new { success = true, trueId = Model.Id, result = Model, imgsCount = 0 });
            }
            return Json(new { success = false, alert = "We're so sorry, but an unexpected error occured. Please, try to send your message a bit later or text us to get more information about this issue" });
        }

        [HttpPost]
        public async Task<IActionResult> Reply(SendReply Model)
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                string? Result = await _messages.ReplyToMessageAsync(Model);
                if (Result != null && Model.Images == null) return Json(new { success = true, trueId = Model.Id, result = Model, imgsCount = 0 });
                else if (Result != null && Model.Images != null) Json(new { success = true, result = Model, trueId = Model.Id, imgUrl = Result, imgsCount = Model.Images.Count });
                else return Json(new { success = false, alert = "This reply cannot be sent" });
            }
            return Json(new { success = false, alert = "We're sorry, but you haven't got access to send a reply" });
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
        public async Task<IActionResult> Edit(SendEdit Model)
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
        public async Task<IActionResult> MarkasRead(int Id, int UserId)
        {
            int Result = await _messages.MarkAsReadAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true });
            else return Json(new { success = false, alert = "This message is already read" });
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
            string? CurrentUserId_Str;
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
