using AppY.Abstractions;
using AppY.Data;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppY.Controllers
{
    public class AnswerController : Controller
    {
        private readonly Context _context;
        private readonly Answer _answer;
        public AnswerController(Context context, Answer answer)
        {
            _context = context;
            _answer = answer;
        }

        [HttpPost]
        public async Task<IActionResult> SendDiscussionMessageAnswer(Answers_ViewModel Model)
        {
            if(ModelState.IsValid && Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (!String.IsNullOrWhiteSpace(CurrentUserId_Str))
                {
                    bool TryParseUserId = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParseUserId)
                    {
                        Model.UserId = UserId;
                        int Result = await _answer.SendAnAnswerAsync(Model);
                        if (Result > 0) return Json(new { success = true, result = Model, id = Result });
                    }
                }
                return Json(new { success = false, alert = "You can't send an answer for this message" });
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error has occurred. Please, try to send your answer later" });
        }

        [HttpPost]
        public async Task<IActionResult> EditDiscussionMessageAnswer(Answers_ViewModel Model)
        {
            if(ModelState.IsValid && Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (!String.IsNullOrWhiteSpace(CurrentUserId_Str))
                {
                    bool TryParseUserId = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParseUserId)
                    {
                        Model.UserId = UserId;
                        int Result = await _answer.EditAnswerAsync(Model);
                        if (Result > 0) return Json(new { success = true, result = Result, text = Model.Text });
                    }
                }
            }
            return Json(new { success = false, alert = "An unexpected error has been occured. Please, try to edit your answer a bit later" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDiscussionMessage(int Id)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (!String.IsNullOrWhiteSpace(CurrentUserId_Str))
                {
                    bool TryParseUserId = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParseUserId)
                    {
                        int Result = await _answer.DeleteAnswerAsync(Id, UserId);
                        if (Result > 0) return Json(new { success = true, result = Result });
                    }
                    return Json(new { success = false, alert = "We're sorry, but you cannot delete this message right now" });
                }
            }
            return Json(new { success = false, alert = "You're unable to edit or delete this answer" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscussionMessageAnswers(int Id, int SkipCount, string MessageText)
        {
            string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
            if (!String.IsNullOrWhiteSpace(CurrentUserId_Str))
            {
                bool TryParseUserId = Int32.TryParse(CurrentUserId_Str, out int UserId);
                if (TryParseUserId)
                {
                    IQueryable<Answers_ViewModel>? Answers_Preview = _answer.GetAnswers(Id, UserId, SkipCount, 75);
                    if (Answers_Preview != null)
                    {
                        List<Answers_ViewModel>? Answers = await Answers_Preview.ToListAsync();
                        if (Answers is not null) return Json(new { success = true, currentUserId = UserId, result = Answers, message = MessageText, count = Answers.Count });
                    }
                }
            }
            return Json(new { success = false, message = MessageText, count = 0 });
        }
    }
}
