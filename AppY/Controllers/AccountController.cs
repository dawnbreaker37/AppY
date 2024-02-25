using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
        public async Task<IActionResult> LogIn(SignIn Model)
        {
            if (ModelState.IsValid)
            {
                bool Result = await _account.SignInAsync(Model);
                if (Result) return Json(new { success = true });
            }
            return Json(new { success = false, alert = "Wrong username/email or password. Please, check all datas and try again" });
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
                        Title = "Your Reserve Code"
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
    }
}
