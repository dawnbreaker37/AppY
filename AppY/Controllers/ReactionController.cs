using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppY.Controllers
{
    public class ReactionController : Controller
    {
        private readonly Context _context;
        private readonly ReactionAbstraction _reaction;

        public ReactionController(Context context, ReactionAbstraction reaction)
        {
            _context = context;
            _reaction = reaction;
        }

        [HttpPost]
        public async Task<IActionResult> SetDiscussionMessageReaction(int Id, int UserId, int ReactionId)
        {
            int Result;
            if (ReactionId > 0) Result = await _reaction.SetReactionAsync(Id, UserId, ReactionId);
            else Result = await _reaction.DeleteReactionAsync(Id, UserId);

            if (Result > 0) return Json(new { success = true, result = Id, reactionsCount = Result });
            else return Json(new { success = false, alert = "Sorry, but you cannot set or delete any discussion for this message" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscussionMessageReactions(int Id)
        {
            IQueryable<IGrouping<int, DiscussionMessageReaction>>? ReactionsPreview = _reaction.GetMessageReactions(Id);
            if(ReactionsPreview != null)
            {
                List<IGrouping<int, DiscussionMessageReaction>>? Reactions = await ReactionsPreview.ToListAsync();
                if (Reactions is not null) return Json(new { success = true, id = Id, result = Reactions, count = Reactions.Count });
            }
            return Json(new { success = false, alert = "No reactions for this message" });
        }
    }
}
