using AppY.Data;
using AppY.Interfaces;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace AppY.Controllers
{
    public class DiscussionMessageController : Controller
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IDiscussionMessage _discussionMessage;

        public DiscussionMessageController(Context context, IMemoryCache memoryCache, IDiscussionMessage discussionMessage)
        {
            _context = context;
            _memoryCache = memoryCache;
            _discussionMessage = discussionMessage;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessage Model)
        {
            if(ModelState.IsValid && User.Identity.IsAuthenticated)
            {              
                int Result = await _discussionMessage.SendMessageAsync(Model);
                if (Result != 0) return Json(new { success = true, result = Model });
            }
            return Json(new { success = false, alert = "An error unexpected so it's unable to send a message" });
        }
    }
}
