using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppY.Controllers
{
    public class SavedMessageController : Controller
    {
        private readonly Context _context;
        private readonly ISavedMessage _savedMessage;

        public SavedMessageController(Context context, ISavedMessage savedMessage)
        {
            _context = context;
            _savedMessage = savedMessage;
        }

        [HttpGet]
        public async Task<IActionResult> IsPinned(int Id, int UserId)
        {
            bool Result = await _savedMessage.IsSavedMessagePinnedAsync(Id, UserId);
            return Json(new { success = Result, id = Id });
        }

        [HttpPost]
        public async Task<IActionResult> Pin(int Id)
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if(TryParse)
                {
                    int Result = await _savedMessage.PinAsync(Id, UserId);
                    if (Result > 0) return Json(new { success = true, id = Result });
                }
            }
            return Json(new { success = false, alert = "This message cannot be pinned" });
        }

        [HttpPost]
        public async Task<IActionResult> Unpin(int Id)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if (TryParse)
                {
                    string? Result = await _savedMessage.UnpinAsync(Id, UserId);
                    return Json(new { success = true, result = Result });
                }
            }
            return Json(new { success = false, alert = "This message cannot be unpinned by you" });
        }

        [HttpGet]
        public async Task<IActionResult> GetPinnedMessageInfo(int Id, int SkipCount)
        {
            SavedMessageContent? Result = await _savedMessage.GetPinnedMessageInfoAsync(Id, SkipCount);
            if (Result != null) return Json(new { success = true, result = Result });
            else return Json(new { success = false, alert = "No information about next pinned message" });
        }

        [HttpPost]
        public async Task<IActionResult> Star(int Id, string? Text)
        {
            string? Result = await _savedMessage.StarSavedMessageAsync(Id, Text);
            if (Result != null) return Json(new { success = true, id = Id, result = Result });
            else return Json(new { success = false, alert = "Unable to star that message" });
        }

        [HttpPost]
        public async Task<IActionResult> Unstar(int Id)
        {
            bool Result = await _savedMessage.UnstarSavedMessageAsync(Id);
            if (Result) return Json(new { success = true, id = Id });
            else return Json(new { success = false, alert = "Unable to unstar that message" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveChatMessage(int Id, int ChatId)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if(TryParse)
                {
                    bool Result = await _savedMessage.SaveTheMessageAsync(Id, ChatId, UserId, true);
                    if (Result) return Json(new { success = true, alert = "Message has been saved in your <span class='fw-500'>saved messages</span>" });
                }
                return Json(new { success = false, alert = "Can't find any information about that message to save. May be it had been recently deleted" });
            }
            return Json(new { success = false, alert = "You've no access to save that message" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveDiscussionMessage(int Id, int DiscussionId)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if(TryParse)
                {
                    bool Result = await _savedMessage.SaveTheMessageAsync(Id, DiscussionId, UserId, false);
                    if (Result) return Json(new { success = true, alert = "Message has been saved" });
                    else return Json(new { success = false, alert = "An unexpected error has been occured. Please, try to save that message a bit later" });
                }
            }
            return Json(new { success = false, alert = "You've no access to save a message from that discussion" });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SavedMessageContent_ViewModel Model)
        {
            if(ModelState.IsValid && Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if (TryParse)
                {
                    Model.UserId = UserId;
                    if (Model.MessageId <= 0)
                    {
                        int Result = await _savedMessage.AddSavedMessageAsync(Model);
                        if (Result > 0)
                        {
                            if (Model.Files != null)
                            {
                                string? ImgResult = await _savedMessage.SendImagesWMessage(Result, Model.Files);
                                return Json(new { success = true, isEdited = false, id = Result, result = Model, filesCount = Model.Files.Count, file = ImgResult });
                            }
                            else return Json(new { success = true, isEdited = false, id = Result, result = Model });
                        }
                        else return Json(new { success = false, alert = "We're sorry, but something unexpected happened. Please, try to send your saved message a bit later" });
                    }
                    else
                    {
                        string? Result = await _savedMessage.EditSavedMessageAsync(Model);
                        if (Result != null) return Json(new { success = true, isEdited = true, text = Result, id = Model.MessageId });
                        else return Json(new { success = false, alert = "We're sorry, but something unexpected happened. Please, try to edit that message a bit later" });
                    }
                }
            }
            return Json(new { success = false, alert = "Unexpected error. Please, reload the page or wait until the error will be fixed" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                if (TryParse)
                {
                    int Result = await _savedMessage.DeleteSavedMessageAsync(Id, UserId);
                    if (Result > 0) return Json(new { success = true, id = Result });
                    else return Json(new { success = false, alert = "We're sorry, but something unexpected happened. Please, try to delete your saved message a bit later" }); ;
                }
            }
            return Json(new { success = false, alert = "You've no access to remove that message" });
        }
    }
}
