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
        private readonly IMailMessages _mailMessages;
        private readonly IChat _chat;
        private readonly ChatMessageAbstraction _messages;
        
        public ChatController(Context context, ChatMessageAbstraction messages, IUser user, IChat chat, IMailMessages mailMessages)
        {
            _context = context;
            _user = user;
            _messages = messages;
            _mailMessages = mailMessages;
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
                                        ChatPasswordSettings? ChatPasswordInfo = await _chat.GetChatPasswordInfoAsync(Id, CurrentUserId);
                                        User? SecondUserInfo = await _user.GetUserSuperShortInfoEvenIfPrivateAsync(SecondUserId);
                                        int CurrentChatUserId = await _context.ChatUsers.AsNoTracking().Where(c => c.UserId == CurrentUserId && c.ChatId == Id).Select(c => c.Id).FirstOrDefaultAsync();
                                        ChatInfo.Name = ChatInfo.Name == null ? "New Chat" : ChatInfo.Name;

                                        if(SecondUserInfo != null) SecondUserInfo.LastSeenText = _user.SetLastSeenText(SecondUserInfo.LastSeen);

                                        if (SentMessagesCount > 0)
                                        {
                                            IQueryable<IGrouping<DateTime, DiscussionMessage>>?  Messages_Preview = _messages.GetMessages(Id, CurrentUserId, 0, 35);
                                            if(Messages_Preview != null) Messages = await Messages_Preview.ToListAsync();
                                        }

                                        ViewBag.UserInfo = UserInfo;
                                        ViewBag.AutodeleteDelayValue = _user.AutodeleteDelay(UserInfo.AreMessagesAutoDeletable);
                                        ViewBag.SecondUserInfo = SecondUserInfo;
                                        ViewBag.ChatInfo = ChatInfo;
                                        ViewBag.PasswordSettings = ChatPasswordInfo;
                                        ViewBag.IsChatLocked = ChatPasswordInfo != null ? true : false;
                                        ViewBag.IsMuted = IsMuted;
                                        ViewBag.DisabledTimeLeft = DisabledTimeLeft;
                                        ViewBag.Messages = Messages;
                                        ViewBag.MessagesCount = SentMessagesCount;
                                        ViewBag.ReceiverId = SecondUserId;
                                        ViewBag.CurrentChatUserId = CurrentChatUserId;

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

        public async Task<IActionResult> SC(int Id)
        {
            if(Id > 0)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    if (CurrentUserId_Str != null)
                    {
                        bool TryToParse = Int32.TryParse(CurrentUserId_Str, out int CurrentUserId);
                        if (TryToParse)
                        {
                            SecretChat? SecretChatInfo = await _chat.GetSecretChatInfoAsync(Id, CurrentUserId);
                            if (SecretChatInfo != null)
                            {
                                User? UserInfo = await _user.GetMainUserInfoAsync(CurrentUserId);
                                User? SecondUserInfo = await _user.GetUserSuperShortInfoAsync(await _chat.GetChatSecondUserInfoAsync(Id, CurrentUserId));

                                ViewBag.ChatInfo = SecretChatInfo;
                                ViewBag.UserInfo = UserInfo;
                                ViewBag.SecondUserInfo = SecondUserInfo;

                                return View();
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CreateSecretChat(int Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId = Request.Cookies["CurrentUserId"];
                    bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                    if (TryParse)
                    {
                        int Result = await _chat.CreateSecretChat(UserId, Id);
                        if (Result > 0) return RedirectToAction("SC", "Chat", new { Id = Result });
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Create", "Account");
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
        public async Task<IActionResult> GetChatsShortly(int ChatId, bool IsForForwarding)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? UserId_Str = Request.Cookies["CurrentUserId"];
                if(UserId_Str != null)
                {
                    bool TryParse = Int32.TryParse(UserId_Str, out int UserId);
                    if(TryParse)
                    {
                        IQueryable<ChatUsers>? Result_Preview = _chat.GetUserChatsShortly(UserId, ChatId);
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

        [HttpPost]
        public async Task<IActionResult> SwitchPreviewOption(int Id, bool Value)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if(CurrentUserId_Str != null)
                {
                    bool TryParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParse)
                    {
                        int Result = await _chat.SwitchPreviewingOptionAsync(Id, UserId, Value);
                        if (Result >= 1 && Result <= 2) return Json(new { success = true, result = Result });
                        else return Json(new { success = false, alert = "You can't edit previewing settings of this chat" });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but something went wrong. Please, try to edit previewing settings a bit later" });
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int Id, int UserId, int SkipCount)
        {
            bool IsAvailableToBePreviewed = await _chat.CheckChatAvailabilityToBeViewed(Id, UserId);
            if (IsAvailableToBePreviewed)
            {
                IQueryable<ChatMessage>? Messages_Preview = _messages.GetMessagesShortly(Id, SkipCount, 65);
                if(Messages_Preview != null)
                {
                    List<ChatMessage>? Messages = await Messages_Preview.ToListAsync();
                    if (Messages != null) return Json(new { success = true, id = Id, userId = UserId, result = Messages, count = Messages.Count, fullCount = SkipCount + Messages.Count });
                }
                return Json(new { success = false, alert = "No more messages to load for previewing" });
            }
            return Json(new { success = false, alert = "This chat cannot be previewed" });
        }

        [HttpPost]
        public async Task<IActionResult> CheckPassword(int Id, string? Password)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (CurrentUserId_Str != null)
                {
                    bool TryParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParse)
                    {
                        bool Result = await _chat.CheckChatPasswordAsync(Id, UserId, Password);
                        if (Result) return Json(new { success = true });
                    }
                }
            }
            return Json(new { success = false, alert = "Incorrect chat password. Try again or receive it to your email" });
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(int Id, string? Password)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (CurrentUserId_Str != null)
                {
                    bool TryParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParse)
                    {
                        bool Result = await _chat.SetPasswordAsync(Id, UserId, Password);
                        if (Result) return Json(new { success = true, alert = "Your password for this chat has been successfully set" });
                    }
                }
            }
            return Json(new { success = false, alert = "You can't set a password for this chat" });
        }

        [HttpPost]
        public async Task<IActionResult> RemovePassword(int Id)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                if (CurrentUserId_Str != null)
                {
                    bool TryParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                    if (TryParse)
                    {
                        bool Result = await _chat.SetPasswordAsync(Id, UserId, null);
                        if (Result) return Json(new { success = true, alert = "Your password for this chat has been removed" });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but something unexpected happened. Please, try to remove your password a bit later" });
        }

        [HttpPost]
        public async Task<IActionResult> SendChatPasswordViaEmail(int Id)
        {
            if (Id > 0)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    if (CurrentUserId_Str != null)
                    {
                        bool TryParse = Int32.TryParse(CurrentUserId_Str, out int UserId);
                        if (TryParse)
                        {
                            bool CheckChatAvailability = await _chat.CheckUserAvailabilityInChat(Id, UserId);
                            if(CheckChatAvailability)
                            {
                                string? ChatPassword = await _context.ChatUsers.AsNoTracking().Where(c => c.ChatId == Id && c.UserId == UserId).Select(c => c.Password).FirstOrDefaultAsync();
                                string? UserMail = await _context.Users.AsNoTracking().Where(u => u.Id == UserId).Select(u => u.Email).FirstOrDefaultAsync();
                                if(ChatPassword != null && UserMail != null)
                                {
                                    SendEmail sendEmail = new SendEmail()
                                    {
                                        SentFrom = "bluejade@mail.ru",
                                        Body = "<h1 style=\"text-align: center;\"><span style='font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233); font-size: 48px;'>" + ChatPassword + "</span></h1>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>Hello! You've recently requested one of your chats password. Please, enter provided password and write down it somewhere in addition to not lost it again <br/>With Love, Bluejade</span>\r\n<hr>\r\n<h3 style=\"text-align: center; line-height: 1;\"><span style='font-size: 24px; font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233);'>Attention</span></h3>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>If this request was not sent by you, immediately enter and change your chat's password</span></p>\r\n<p style=\"text-align: center;\"><br></p>\r\n<p style=\"text-align: center;\"><span style=\"text-align: inherit;\"><span style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\">&nbsp;</span></span></p>",
                                        SendTo = UserMail,
                                        Subject = "Chat Password Request",
                                        Title = "Bluejade"
                                    };
                                    bool Result = await _mailMessages.SendMessageAsync(new MailKitModel(), sendEmail);
                                    if (Result) return Json(new { success = true, alert = "Chat password has been sent to your inbox" });
                                }
                            }
                        }
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but something unexpected happened. Please, try to get your chat's password again a bit later" });
        }

        [HttpGet]
        public async Task<IActionResult> CheckChatAvailability(int UserId1, int UserId2)
        {
            int Result = await _chat.FindChatAvailability(UserId1, UserId2, true);
            if (Result > 0) return Json(new { success = true, result = Result });
            else return Json(new { success = false, alert = "We're sorry, but an unexpected error has occured. Please, try message this user a bit later" });
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
