using AppY.Abstractions;
using AppY.ChatHub;
using AppY.Data;
using AppY.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AppY.Controllers
{
    public class ChatMessageController : Controller
    {
        private readonly Context _context;
        private readonly ChatMessageAbstraction _message;
        private readonly IHubContext<MessagesHub> _hubContext;

        public ChatMessageController(Context context, ChatMessageAbstraction message, IHubContext<MessagesHub> hubContext)
        {
            _context = context;
            _message = message;
            _hubContext = hubContext;
        }

        //[HttpPost]
        //public async Task<IActionResult> SendMessage(string? Text, IFormFileCollection Files, int SenderId, int ReceiverId, int ChatId, int IsAutodeletable, string? Chatname, int CurrentChatUserId)
        //{
        //    return Ok();
        //}

        [HttpGet]
        public async Task<IActionResult> IsMessagePinned(int Id)
        {
            bool Result = await _message.IsPinnedAsync(Id);
            return Json(new { success = true, result = Result, id = Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetPinnedMessageInfo(int Id, int SkipCount)
        {
            DiscussionMessage? PinnedMesageInfo = await _message.GetPinnedMessageInfoAsync(Id, SkipCount);
            if (PinnedMesageInfo != null) return Json(new { success = true, result = PinnedMesageInfo, skipCount = SkipCount });
            else return Json(new { success = false, alert = "Unable to get any info about next pinned message" });
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageInfo(int Id, int UserId)
        {
            DiscussionMessage? Result = await _message.GetMessageInfoAsync(Id, UserId);
            if (Result != null)
            {
                if(DateTime.Now.Subtract(Result.SentAt).TotalDays > 3) return Json(new { success = true, isEditable = false, userId = UserId, result = Result });
                else return Json(new { success = true, isEditable = true, userId = UserId, result = Result });
            }
            else return Json(new { success = false, alert = "Unable to get any info about that message" });
        }
    }
}
