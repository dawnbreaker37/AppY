using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppY.Controllers
{
    public class ChatMessageController : Controller
    {
        private readonly Context _context;
        private readonly ChatMessageAbstraction _message;

        public ChatMessageController(Context context, ChatMessageAbstraction message)
        {
            _context = context;
            _message = message;
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
