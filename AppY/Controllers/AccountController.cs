using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace AppY.Controllers
{
    public class AccountController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUser _user;
        private readonly IAccount _account;
        private readonly IMailMessages _mailMessages;
        private readonly IMemoryCache _memoryCache;

        public AccountController(Context context, UserManager<User> userManager, SignInManager<User> signInManager, IUser user, IAccount account, IMailMessages mailMessages, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _user = user;
            _account = account;
            _mailMessages = mailMessages;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUp Model)
        {
            if (ModelState.IsValid)
            {
                (int, string?)? Results = await _account.SignUpAsync(Model);
                if (Results.Value.Item1 != 0) return Json(new { success = true, alert = "You've been successfully signed up, congrats! Please, read well all information in widget below and write down somewhere the reserve code. Good luck!", reserveCode = Results.Value.Item2, id = Results.Value.Item1 });
                else return Json(new { success = false, error = Results.Value.Item2, alert = "Something went wrong: " });
            }
            else return Json(new { success = false, alert = "Something is wrong with one of your entered datas. Please, check all of them and then try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            if(User.Identity.IsAuthenticated)
            {
                if(Request.Cookies.ContainsKey("CurrentUserId")) Response.Cookies.Delete("CurrentUserId");
                await _signInManager.SignOutAsync();
                return RedirectToAction("Create", "Account");
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(SignIn Model)
        {
            if (ModelState.IsValid)
            {
                SignInSuccess? Result = await _account.SignInAsync(Model);
                if (Result != null) return Json(new { success = true, result = Result });
            }
            return Json(new { success = false, alert = "Wrong username/email or password. Please, check all datas and try again" });
        }

        [HttpPost]
        public async Task<IActionResult> TFASignIn(string Email, string? Password, string? Code)
        {
            User? UserInfo = await _userManager.FindByEmailAsync(Email);
            if(UserInfo != null)
            {
                bool Result = await _account.TFASignInAsync(UserInfo, Password, Code);
                if (Result) return Json(new { success = true });
            }
            return Json(new { success = false, alert = "Wrong code. Please, check your email once again" });
        }

        [HttpPost]
        public async Task<IActionResult> EasyEntry(int Id)
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int UserId);
                if (TryParse)
                {
                    bool Result = await _account.EasyEntryAsync(UserId, Id);
                    if (Result) return RedirectToAction("Account", "User");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SendSingleUseCode(string Email)
        {
            if(ModelState.IsValid)
            {
                string? OneTimeCode = Guid.NewGuid().ToString("D").Substring(0, 8);
                if(!String.IsNullOrEmpty(OneTimeCode))
                {
                    MailKitModel mailKitModel = new MailKitModel();
                    SendEmail SendEmailModel = new SendEmail()
                    {
                        Body = "<h1 style=\"text-align: center;\"><span style='font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233); font-size: 48px;'>" + OneTimeCode + "</span></h1>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>Hi, there! You&apos;ve recently tried to get a one-time code to recover your account password, so here&nbsp;</span><span style=\"text-align: inherit;\">\r\n        <font face=\"Trebuchet MS, Helvetica, sans-serif\"><span style=\"font-size: 20px;\">is it. Use this to make it clear that you are the owner of&nbsp;the&nbsp;account. After that&nbsp;you&apos;ll be able to create a brand-new password for your account, good luck!</span></font>\r\n    </span></p>\r\n<hr>\r\n<h3 style=\"text-align: center; line-height: 1;\"><span style='font-size: 24px; font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233);'>Attention</span></h3>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>If this request was not sent by, please just ignore this message. Thank You</span></p>\r\n<p style=\"text-align: center;\"><br></p>\r\n<p style=\"text-align: center;\"><span style=\"text-align: inherit;\"><span style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\">&nbsp;</span></span></p>",
                        SendTo = Email,
                        SentFrom = "bluejade@mail.ru",
                        Subject = "Password Recovery",
                        Title = "Password Recovery Code"
                    };
                    bool Result = await _mailMessages.SendMessageAsync(mailKitModel, SendEmailModel);
                    if(Result)
                    {
                        _memoryCache.Remove(Email + "_singleUseCode");
                        _memoryCache.Set(Email + "_singleUseCode", OneTimeCode, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(12)));

                        return Json(new { success = true, email = Email, alert = "Single-use code has just been sent to your email. Please, check your email and enter that code to make it clear that you're the owner of this account. <br/>The sent code will be active within <span class='fw-500 text-neon-purple'>12</span> mins " });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but an unexpected error has been occured. Please, try this action again later" });
        }

        [HttpPost]
        public async Task<IActionResult> SendReserveCodeViaEmail(string Email)
        {
            if(!String.IsNullOrEmpty(Email))
            {
                string? ReserveCode = await _user.GetReserveCodeViaEmailAsync(Email);
                if(ReserveCode != null)
                {
                    SendEmail Model = new SendEmail
                    {
                        Body = "<h1 style=\"text-align: center;\"><span style='font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233); font-size: 48px;'>" + ReserveCode + "</span></h1>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>Hi, there! You&apos;ve recently tried to get a one-time code to recover your account password, so here&nbsp;</span><span style=\"text-align: inherit;\">\r\n        <font face=\"Trebuchet MS, Helvetica, sans-serif\"><span style=\"font-size: 20px;\">is it. Use this to make it clear that you are the owner of&nbsp;the&nbsp;account. After that&nbsp;you&apos;ll be able to create a brand-new password for your account, good luck!</span></font>\r\n    </span></p>\r\n<hr>\r\n<h3 style=\"text-align: center; line-height: 1;\"><span style='font-size: 24px; font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233);'>Attention</span></h3>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>If this request was not sent by, please just ignore this message. Thank You</span></p>\r\n<p style=\"text-align: center;\"><br></p>\r\n<p style=\"text-align: center;\"><span style=\"text-align: inherit;\"><span style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\">&nbsp;</span></span></p>",
                        SendTo = Email,
                        SentFrom = "bluejade@mail.ru",
                        Subject = "Account Reserve Code",
                        Title = "Bluejade"
                    };
                    bool MailMessageSentResult = await _mailMessages.SendMessageAsync(new MailKitModel(), Model);
                    if (MailMessageSentResult)
                    {
                        _memoryCache.Remove(Email + "_reserveCode");
                        _memoryCache.Set(Email + "_reserveCode", ReserveCode, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(12)));

                        return Json(new { success = true, alert = "Your <span class='fw-500'>reserve code</span> has been successfully sent to your email. Please, check your inbox and enter that code here to make sure that you're the owner of this account. Thank You" });
                    }
                    else return Json(new { success = true, alert = "We're sorry, but an unexpected error occured. Please, try again later" });
                }
                else return Json(new { success = false, alert = "We're sorry, but haven't get any account bound created by this email" });
            }
            return Json(new { success = false, alert = "Wrong email address. Unable to send a message to this email" });
        }

        [HttpPost]
        public async Task<IActionResult> SendTwoFactorAuthenticationCode(string Email, bool AlternativeText)
        {
            if(Email != null)
            {
                User? UserInfo = await _userManager.FindByEmailAsync(Email);
                if(UserInfo != null)
                {
                    string? Code = Guid.NewGuid().ToString("N").Substring(4, 12);
                    string? TwoFactorToken = await _userManager.GenerateTwoFactorTokenAsync(UserInfo, TokenOptions.DefaultProvider);
                    string? BodyText;
                    if (AlternativeText) BodyText = "<h1 style=\"text-align: center;\"><span style='font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233); font-size: 48px;'>" + Code + "</span></h1>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>Hello! This is your two-factor authentication code. Please enter the provided code to sign-in into your account. This code is active only 12 minutes. Lately, you'll need a new one</span>\r\n<hr>\r\n<h3 style=\"text-align: center; line-height: 1;\"><span style='font-size: 24px; font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233);'>Attention</span></h3>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>If this request was not sent by you, immediately enter and change your account's password. Thank You</span></p>\r\n<p style=\"text-align: center;\"><br></p>\r\n<p style=\"text-align: center;\"><span style=\"text-align: inherit;\"><span style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\">&nbsp;</span></span></p>";
                    else BodyText = "<h1 style=\"text-align: center;\"><span style='font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233); font-size: 48px;'>" + Code + "</span></h1>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>Hello! This is your two-factor verification code. Please enter the provided code to enable two-factor verification. This code is active only 12 minutes. Lately, you'll need a new one to enable your two-factor authentication</span>\r\n<hr>\r\n<h3 style=\"text-align: center; line-height: 1;\"><span style='font-size: 24px; font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233);'>Attention</span></h3>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>If this request was not sent by you, immediately enter and change your account's password. Thank You</span></p>\r\n<p style=\"text-align: center;\"><br></p>\r\n<p style=\"text-align: center;\"><span style=\"text-align: inherit;\"><span style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\">&nbsp;</span></span></p>";
                    SendEmail Model = new SendEmail()
                    {
                        SendTo = Email,
                        SentFrom = "bluejade@mail.ru",
                        Title = "BlueJade",
                        Subject = "Two-Factor Authentication Enabling Code",
                        Body = BodyText
                    };

                    bool MailMsgSentResult = await _mailMessages.SendMessageAsync(new MailKitModel(), Model);
                    if (MailMsgSentResult)
                    {
                        _memoryCache.Remove(Email + "_singleUseCode");
                        _memoryCache.Remove(Email + "_2FAtoken");
                        _memoryCache.Set(Email + "_singleUseCode", Code, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(12)));
                        _memoryCache.Set(Email + "_2FAtoken", TwoFactorToken, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(12)));
                        return Json(new { success = true, alert = "A verification code has been sent to your email. Please, check your mail and set that code in the form below" });
                    }
                }
            }
            return Json(new { success = false, alert = "We're sorry, but we can't send a code to your email due to some error. Please, try to enable your verification later" });
        }

        [HttpPost]
        public async Task<IActionResult> Send2FADisablingCode(string Email)
        {
            if(Email != null)
            {
                User? UserInfo = await _userManager.FindByEmailAsync(Email);
                if(UserInfo != null)
                {
                    string? Code = Guid.NewGuid().ToString("N").Substring(6, 8);
                    string? Token = await _userManager.GenerateTwoFactorTokenAsync(UserInfo, TokenOptions.DefaultProvider);
                    SendEmail sendEmail = new SendEmail()
                    {
                        Body = "<h1 style=\"text-align: center;\"><span style='font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233); font-size: 48px;'>" + Code + "</span></h1>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>Hello! Here's your Two-Step Verification Code. Please, input the given code to deactivate Two-Step Verification for your account. Please be aware that your account may become more susceptible to unauthorized access without 2FA. This code is active only 12 minutes. Lately, you'll need a new one to enable your two-factor authentication</span>\r\n<hr>\r\n<h3 style=\"text-align: center; line-height: 1;\"><span style='font-size: 24px; font-family: \"Trebuchet MS\", Helvetica, sans-serif; color: rgb(134, 91, 233);'>Attention</span></h3>\r\n<p style=\"text-align: center;\"><span style='font-size: 20px; font-family: \"Trebuchet MS\", Helvetica, sans-serif;'>If this request was not sent by you, immediately enter and change your account's password. Thank You</span></p>\r\n<p style=\"text-align: center;\"><br></p>\r\n<p style=\"text-align: center;\"><span style=\"text-align: inherit;\"><span style=\"font-family: 'Trebuchet MS', Helvetica, sans-serif;\">&nbsp;</span></span></p>",
                        SentFrom = "bluejade@mail.ru",
                        SendTo = Email,
                        Subject = "2FA Disabling Code",
                        Title = "Bluejade"
                    };
                    bool Result = await _mailMessages.SendMessageAsync(new MailKitModel(), sendEmail);
                    if(Result)
                    {
                        _memoryCache.Remove(Email + "_singleUseCode");
                        _memoryCache.Remove(Email + "_singleUseToken");
                        _memoryCache.Set(Email + "_singleUseCode", Code);
                        _memoryCache.Set(Email + "_singleUseToken", Token);

                        return Json(new { success = true, alert = "A 2FA disabling code has been sent to your inbox. Please, check your inbox and enter sent code here to disable your 2FA" });
                    }
                }
            }
            return Json(new { success = false, alert = "Unable to send a message to your email. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication(string Email, string Code)
        {
            bool EnableResult = await _account.Enable2FAAsync(Email, Code);
            if (EnableResult) return Json(new { success = true, alert = "Well done! You've successfully enabled 2FA for your account" });
            return Json(new { success = false, alert = "Something went wrong. May be wrong code or something else" });
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorAuthentication(string? Email, string? Code)
        {
            bool Result = await _account.Disable2FAAsync(Email, Code);
            if (Result) return Json(new { success = true, alert = "2FA for your account has been disabled" });
            else return Json(new { success = false, alert = "Wrong verification code. Please, check your inbox again" });
        }

        [HttpGet]
        public async Task<IActionResult> SubmitSingleUsingCode(string Email, string Code)
        {
            string? Result = await _user.SubmitSingleUseCodeAsync(Email, Code);
            if (!String.IsNullOrEmpty(Result)) return Json(new { success = true, email = Email, token = Result, alert = "Well done! Now, create your brand-new password" });
            else return Json(new { success = false, alert = "Wrong email or code. Please, check all datas and try again" });
        }

        [HttpGet]
        public async Task<IActionResult> SubmitAccountViaReserveCode(string Email, string ReserveCode)
        {
            string? Result = await _user.SubmitReserveCodeViaEmailAsync(Email, ReserveCode);
            if (!String.IsNullOrEmpty(Result)) return Json(new { success = true, email = Email, token = Result, alert = "Well done! Now, lets create a brand-new password for your account" });
            else return Json(new { success = false, alert = "Wrong email or reserve code. Please, check all datas and try again" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePassword Model)
        {
            if(ModelState.IsValid)
            {
                bool Result = await _account.UpdatePasswordAsync(Model);
                if (Result) return Json(new { success = true, alert = "You've successfully updated your password. Congratulations!" });
            }
            return Json(new { success = false, alert = "Unable to reset your password. Please, try again later" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword Model)
        {
            if (ModelState.IsValid)
            {
                bool Result = await _account.ResetPasswordAsync(Model);
                if (Result) return Json(new { success = true, alert = "You've successfully changed your password", date = DateTime.Now });
            }
            return Json(new { success = false, alert = "Something went wrong. Please, check all datas and then try again" });
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailUnique(string? Email)
        {
            bool Result = await _user.IsEmailUniqueAsync(Email);
            if (Result) return Json(new { success = true, value = Email });
            else return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> IsUsernameUnique(string? Username)
        {
            bool Result = await _user.IsUsernameUniqueAsync(Username);
            if (Result) return Json(new { success = true, value = Username });
            else return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> CheckAccountCredentials(string? Email, string? Password, int CurrentUserId)
        {
            int Result = await _account.CheckAccountCredentialsAsync(Email, Password);
            if (Result > 0 && Result != CurrentUserId) return Json(new { success = true, account = Email, id = Result, alert = "Account credentials are true. Let's get to the next step" });
            else return Json(new { success = false, alert = "Account credentials are wrong" });
        }

        [HttpPost]
        public async Task<IActionResult> LinkAccounts(int Id, string? Codename)
        {
            if(Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int Id1);
                if(TryParse)
                {
                    User? Result = await _account.LinkAccountsAsync(Id1, Id, Codename);
                    if (Result != null) return Json(new { success = true, result = Result, alert = "Accounts has been successfully linked to each other" });
                    else return Json(new { success = false, alert = "Sorry, but these accounts're already linked to each other" });
                }
            }
            return Json(new { success = false, alert = "Sorry, but an unexpected error has been occured. Please, try to link your accounts again later" });
        }

        [HttpPost]
        public async Task<IActionResult> Unlink(int Id)
        {
            if (Request.Cookies.ContainsKey("CurrentUserId"))
            {
                string? CurrentUserId = Request.Cookies["CurrentUserId"];
                bool TryParse = Int32.TryParse(CurrentUserId, out int Id1);
                if(TryParse)
                {
                    int Result = await _account.UnlinkAccountsAsync(Id1, Id);
                    if (Result > 0) return Json(new { success = true, id = Id, alert = "Accounts has been successfully unlinked" });
                    else return Json(new { success = false, alert = "You can't unlink these accounts" });
                }
            }
            return Json(new { success = false, alert = "We're sorry, but something went wrong. Please, try to unlink your accounts later" });
         }

        public async Task<IActionResult> LinkedAccounts()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Request.Cookies.ContainsKey("CurrentUserId"))
                {
                    string? CurrentUserId_Str = Request.Cookies["CurrentUserId"];
                    if (CurrentUserId_Str != null)
                    {
                        bool TryParse = Int32.TryParse(CurrentUserId_Str, out int CurrentUserId);
                        if (TryParse)
                        {
                            User? UserInfo = await _user.GetMainUserInfoAsync(CurrentUserId);
                            if (UserInfo != null)
                            {
                                List<LinkedAccount_ViewModel>? LinkedAccounts = null;
                                IQueryable<LinkedAccount_ViewModel>? LinkedAccounts_Preview = _account.GetLinkedAccounts(CurrentUserId);
                                if (LinkedAccounts_Preview != null) LinkedAccounts = await LinkedAccounts_Preview.ToListAsync();
                                if (LinkedAccounts != null) LinkedAccounts.ForEach(l => l.LastSeenText = _user.SetLastSeenText(l.LastSeen));

                                ViewBag.UserInfo = UserInfo;
                                ViewBag.LinkedAccounts = LinkedAccounts;
                                ViewBag.LinkedAccountsCount = LinkedAccounts?.Count;

                                return View();
                            }
                        }
                    }
                }
                return RedirectToAction("Create", "Account");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
