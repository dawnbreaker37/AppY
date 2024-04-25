using AppY.Abstractions;
using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppY.Controllers
{
    public class ChatController : Controller
    {
        private readonly Context _context;
        private readonly IUser _user;
        private readonly IChat _chat;
        private readonly ChatMessageAbstraction _messages;
        
        public ChatController(Context context, ChatMessageAbstraction messages, IUser user, IChat chat)
        {
            _context = context;
            _user = user;
            _messages = messages;
            _chat = chat;
        }

        public async Task<IActionResult> C(int Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    if (CurrentUserId_Str != null)
                    {
                        bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int CurrentUserId);
                        if (TryToParse)
                        {
                            User? UserInfo = await _user.GetMainUserInfoAsync(CurrentUserId);
                            if (UserInfo != null)
                            {
                                bool UserAvailability = await _chat.CheckUserAvailabilityInChat(Id, CurrentUserId);
                                if (UserAvailability)
                                {
                                    Chat? ChatInfo = await _chat.GetChatInfoAsync(Id);
                                    if(ChatInfo != null)
                                    {
                                        string? DisabledTimeLeft = null;
                                        List<IGrouping<DateTime, DiscussionMessage>>? Messages = null;
                                        int SentMessagesCount = await _messages.SentMessagesCountAsync(Id);
                                        int SecondUserId = await _chat.ChatSecondUserIdAsync(Id, CurrentUserId);
                                        bool IsMuted = await _chat.IsChatMutedAsync(Id, CurrentUserId);
                                        User? SecondUserInfo = await _user.GetUserSuperShortInfoEvenIfPrivateAsync(SecondUserId);
                                        ChatInfo.Name = ChatInfo.Name == null ? "New Chat" : ChatInfo.Name;

                                        if(SecondUserInfo != null) SecondUserInfo.LastSeenText = _user.SetLastSeenText(SecondUserInfo.LastSeen);

                                        if (SentMessagesCount > 0)
                                        {
                                            IQueryable<IGrouping<DateTime, DiscussionMessage>>?  Messages_Preview = _messages.GetMessages(Id, CurrentUserId, 0, 35);
                                            if(Messages_Preview != null) Messages = await Messages_Preview.ToListAsync();
                                        }

                                        ViewBag.UserInfo = UserInfo;
                                        ViewBag.SecondUserInfo = SecondUserInfo;
                                        ViewBag.ChatInfo = ChatInfo;
                                        ViewBag.IsMuted = IsMuted;
                                        ViewBag.DisabledTimeLeft = DisabledTimeLeft;
                                        ViewBag.Messages = Messages;
                                        ViewBag.MessagesCount = SentMessagesCount;
                                        ViewBag.ReceiverId = SecondUserId;

                                        return View();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Json(new { success = false, alert = "You have no access to this chat" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Chat_ViewModel Model)
        {
            if(ModelState.IsValid)
            {
                int Result = await _chat.EditChatInfoAsync(Model);
                if (Result > 0) return Json(new { success = true, alert = "Chat info edited successfully", result = Model });
            }
            return Json(new { success = false, alert = "Sorry, but you can't edit this chat's settings" });
        }

        [HttpPost]
        public async Task<IActionResult> ClearHistory(int Id, int UserId)
        {
            int Result = await _chat.ClearChatHistoryAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true });
            else return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int Id, int UserId)
        {
            int Result = await _chat.RestoreChatAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, result = Result });
            else return Json(new { success = false, alert = "You can't restore that chat" });
        }

        [HttpPost]
        public async Task<IActionResult> Mute(int Id, int UserId)
        {
            int Result = await _chat.MuteTheChatAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, id = Id, alert = "Chat has been muted" });
            else return Json(new { success = false, alert = "Sorry, but you can't mute this chat" });
        }

        [HttpPost]
        public async Task<IActionResult> Unmute(int Id, int UserId)
        {
            int Result = await _chat.UnmuteTheChatAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, id = Id, alert = "Chat has been unmuted" });
            else return Json(new { success = false, alert = "We're sorry, but an unexpected error has occured. Please, try to mute this chat a bit later" }); ;
        }

        [HttpPost]
        public async Task<IActionResult> Pin(int Id, int UserId)
        {
            int Result = await _chat.PinTheChatAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, alert = "Chat has been successfully pinned", id = Id });
            else return Json(new { success = false, alert = "Sorry, but you can't pin this chat in your chatlist" });
        }

        [HttpPost]
        public async Task<IActionResult> Unpin(int Id, int UserId)
        {
            int Result = await _chat.UnpinTheChatAsync(Id, UserId);
            if (Result > 0) return Json(new { success = true, alert = "Chat has been successfully unpinned", id = Id });
            else return Json(new { success = false, alert = "Sorry, but an unexpected error has occured. Please, try this action a bit later" });
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? Id_Str = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(Id_Str, out int Id);
                if(TryParse)
                {
                    IQueryable<ChatUsers>? Result_Preview = _chat.GetUserChats(Id);
                    if (Result_Preview != null)
                    {
                        List<ChatUsers>? Result = await Result_Preview.ToListAsync();
                        if (Result != null) return Json(new { success = true, result = Result, count = Result.Count });
                    }
                }
            }
            return Json(new { success = false, alert = "No chats to load" });
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsShortly(bool IsForForwarding)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? UserId_Str = Request.Cookies["CurrentUserId"];
                if(UserId_Str != null)
                {
                    bool TryParse = Int32.TryParse(UserId_Str, out int UserId);
                    if(TryParse)
                    {
                        IQueryable<ChatUsers>? Result_Preview = _chat.GetUserChatsShortly(UserId);
                        if (Result_Preview != null)
                        {
                            List<ChatUsers>? Result = await Result_Preview.ToListAsync();
                            if (Result != null) return Json(new { success = true, isForForwarding = IsForForwarding, result = Result, count = Result.Count });
                            else return Json(new { success = false, count = 0, alert = "No Chats to Load" });
                        }
                        return Json(new { success = false, count = 0, alert = "No Chats to Load" });
                    }
                }
            }
            return Json(new { success = false, alert = "Sorry, but we can't find any info about your chats" });
        }

        [HttpGet]
        public async Task<IActionResult> CheckShortnameAvailability(int Id, string? Value)
        {
            bool Result = await _chat.CheckShortnameAvailability(Id, Value);
            if (Result) return Json(new { success = true });
            else return Json(new { success = false });
        }
    }
}
