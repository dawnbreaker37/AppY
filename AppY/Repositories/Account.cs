using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AppY.Repositories
{
    public class Account : Base<User>, IAccount
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUser _userRepository;
        private readonly INotification _notification;
        private readonly IMemoryCache _memoryCache;

        public Account(Context context, UserManager<User> userManager, SignInManager<User> signInManager, IUser userRepository, INotification notification, IMemoryCache memoryCache) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _notification = notification;
            _memoryCache = memoryCache;
        }

        public IQueryable<LinkedAccount_ViewModel>? GetLinkedAccounts(int Id)
        {
            if (Id > 0) return _context.LinkedAccounts.AsNoTracking().Where(l => l.UserId == Id && !l.IsDeleted).Select(l => new LinkedAccount_ViewModel { Id = l.LinkedUserId, CodeName = l.CodeName,  Pseudoname = l.User!.PseudoName, LastSeen = l.User!.LastSeen });
            else return null;
        }

        public async Task<int> CheckAccountCredentialsAsync(string? Email, string? Password)
        {
            if(!String.IsNullOrWhiteSpace(Email) && !String.IsNullOrWhiteSpace(Password))
            {
                User? UserInfo = await _userManager.FindByEmailAsync(Email);
                if(UserInfo != null)
                {
                    bool Result = await _userManager.CheckPasswordAsync(UserInfo, Password);
                    if (Result) return UserInfo.Id;
                }
            }
            return 0;
        }

        public async Task<bool> ResetPasswordAsync(ChangePassword Model)
        {
            if(!String.IsNullOrEmpty(Model.OldPassword) && !String.IsNullOrEmpty(Model.NewPassword) && !String.IsNullOrEmpty(Model.ConfirmPassword) && Model.Id != 0)
            {
                User? UserInfo = await _userManager.FindByIdAsync(Model.Id.ToString());
                if(UserInfo != null)
                {
                    IdentityResult? Result = await _userManager.ChangePasswordAsync(UserInfo, Model.OldPassword, Model.NewPassword);
                    if(Result != null && Result.Succeeded)
                    {
                        Notifications_ViewModel notificationModel = new Notifications_ViewModel
                        {
                            Title = "Password Reseted",
                            Description = "Your account's password has been successfully reseted at " + DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString() + ". Please, check your email for more information",
                            IsUntouchable = true,
                            NotificationCategoryId = 1,
                            IsPinned = false,
                            UserId = UserInfo.Id
                        };

                        //await _notification.SendNotificationAsync(notificationModel);
                        await _context.Users.AsNoTracking().Where(u => u.Id == Model.Id && !u.IsDisabled).ExecuteUpdateAsync(u => u.SetProperty(u => u.PasswordChanged, DateTime.Now));
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<SignInSuccess?> SignInAsync(SignIn SignInModel)
        {
            if(SignInModel != null && (!String.IsNullOrEmpty(SignInModel.Email) && !String.IsNullOrEmpty(SignInModel.Password)))
            {
                User? UserInfo;
                SignInResult? Result;

                if (SignInModel.IsViaUsername) UserInfo = await _userManager.FindByNameAsync(SignInModel.Email);
                else UserInfo = await _userManager.FindByEmailAsync(SignInModel.Email);

                if(UserInfo != null)
                {
                    if (!UserInfo.TwoFactorEnabled) Result = await _signInManager.PasswordSignInAsync(UserInfo, SignInModel.Password, true, true);
                    else Result = await _signInManager.CheckPasswordSignInAsync(UserInfo, SignInModel.Password, true);

                    if (Result != null && Result.Succeeded)
                    {
                        if (UserInfo.TwoFactorEnabled) return new SignInSuccess() { Email = UserInfo.Email, Result = 1, Password = SignInModel.Password };
                        else return new SignInSuccess() { Email = null, Result = 2, Password = null };
                    }
                }
            }
            return null;
        }

        public async Task<bool> TFASignInAsync(User? User, string? Password, string? Code)
        {
            if(User != null && Code != null)
            {
                bool TryGetValue = _memoryCache.TryGetValue(User.Email + "_singleUseCode", out string? CacheCode);
                if(TryGetValue && CacheCode != null && CacheCode.Equals(Code))
                {
                    SignInResult? Result = await _signInManager.PasswordSignInAsync(User, Password, true, true);
                    if(Result.Succeeded) return true;
                }
            }
            return false;
        }

        public async Task<(int, string?)?> SignUpAsync(SignUp SignUpModel)
        {
            if(SignUpModel != null && !String.IsNullOrEmpty(SignUpModel.Password) && !String.IsNullOrEmpty(SignUpModel.Username))
            {
                bool IsEmailUnique = await _userRepository.IsEmailUniqueAsync(SignUpModel.Email);
                bool IsUsernameUnique = await _userRepository.IsUsernameUniqueAsync(SignUpModel.Username);

                if (!IsEmailUnique && !IsUsernameUnique)
                {
                    User NewUser = new User
                    {
                        IsDisabled = false,
                        CreatedAt = DateTime.Now,
                        Email = SignUpModel.Email,
                        PseudoName = SignUpModel.Username,
                        UserName = SignUpModel.Username,
                        EcoModeOnAt = 20,
                        HideLastSeenInfo = false,
                        AvatarBgColor = "F0F0F0",
                        AvatarFgColor = "000000",
                        ShortName = "id" + Guid.NewGuid().ToString("D").Substring(0, 7),
                        ReserveCode = Guid.NewGuid().ToString("D").Substring(0, 6)
                    };
                    IdentityResult? Result = await _userManager.CreateAsync(NewUser, SignUpModel.Password);
                    if (Result.Succeeded)
                    {
                        SignInResult? SignInResultVal = await _signInManager.PasswordSignInAsync(SignUpModel.Username, SignUpModel.Password, true, true);
                        if (SignInResultVal.Succeeded) return (NewUser.Id, NewUser.ReserveCode);
                    }
                    else return (0, Result.Errors.FirstOrDefault()?.Description);
                }
            }
            return (0, null);
        }

        public async Task<bool> UpdatePasswordAsync(UpdatePassword Model)
        {
            if(!String.IsNullOrEmpty(Model.Email) && !String.IsNullOrEmpty(Model.Token) && !String.IsNullOrEmpty(Model.ConfirmPassword) && Model.Password == Model.ConfirmPassword)
            {
                User? UserInfo = await _userManager.FindByEmailAsync(Model.Email);
                if(UserInfo != null)
                {
                    IdentityResult? Result = await _userManager.ResetPasswordAsync(UserInfo, Model.Token, Model.Password);
                    if(Result != null && Result.Succeeded)
                    {
                        Notifications_ViewModel notificationModel = new Notifications_ViewModel
                        {
                            Title = "Password Updated",
                            Description = "Your account's password has been successfully updated at " + DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString() + ". Please, check your email for more information. <br/> <span class='fw-500'>Attention!</span> If this action was not triggered by you, immediately restore your password from <a href='/Account/Create' class='text-decoration-none text-primary'>sign up</a> page",
                            IsUntouchable = true,
                            NotificationCategoryId = 1,
                            UserId = UserInfo.Id
                        };
                        //await _notification.SendNotificationAsync(notificationModel);
                        await _context.Users.AsNoTracking().Where(u => u.Email == Model.Email && !u.IsDisabled).ExecuteUpdateAsync(u => u.SetProperty(u => u.PasswordChanged, DateTime.Now));
                        //await _context.SaveChangesAsync();

                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<User?> LinkAccountsAsync(int Id1, int Id2, string? Codename)
        {
            if(Id1 > 0 && Id2 > 0)
            {
                bool CheckAvailability = await _context.LinkedAccounts.AsNoTracking().AnyAsync(l => l.UserId == Id1 && l.LinkedUserId == Id2);
                bool CheckSecondAccAvailability = await _context.LinkedAccounts.AsNoTracking().AnyAsync(l => l.UserId == Id2 && l.LinkedUserId == Id1);
                if (CheckAvailability || CheckAvailability) return null;
                else
                {
                    LinkedAccount linkedAccount = new LinkedAccount
                    {
                        UserId = Id2,
                        LinkedUserId = Id1,
                        IsDeleted = false,
                        CodeName = Codename
                    };
                    LinkedAccount linkedAccount2 = new LinkedAccount
                    {
                        UserId = Id1,
                        LinkedUserId = Id2,
                        IsDeleted = false,
                        CodeName = Codename
                    };

                    await _context.AddAsync(linkedAccount);
                    await _context.AddAsync(linkedAccount2);
                    await _context.SaveChangesAsync();

                    return await _context.Users.AsNoTracking().Where(u => u.Id == Id2).Select(u => new User { Id = u.Id, PseudoName = u.PseudoName, LastSeen = u.LastSeen }).FirstOrDefaultAsync();
                }
            }
            return null;
        }

        public async Task<int> UnlinkAccountsAsync(int Id1, int Id2)
        {
            if(Id1 > 0 && Id2 > 0)
            {
                int Result = await _context.LinkedAccounts.AsNoTracking().Where(l => l.UserId == Id1 && l.LinkedUserId == Id2).ExecuteDeleteAsync();
                if(Result > 0)
                {
                    await _context.LinkedAccounts.AsNoTracking().Where(l => l.UserId == Id2 && l.LinkedUserId == Id1).ExecuteDeleteAsync();
                    return Id2;
                }
            }
            return 0;
        }

        public async Task<bool> EasyEntryAsync(int Id1, int Id2)
        {
            if(Id1 > 0 && Id2 > 0)
            {
                bool CheckAccountsLinkInfo = await _context.LinkedAccounts.AnyAsync(l => l.UserId == Id1 && l.LinkedUserId == Id2);
                bool CheckSecondAccountAvailability = await _context.LinkedAccounts.AnyAsync(l => l.UserId == Id2 && l.LinkedUserId == Id1);
                if(CheckAccountsLinkInfo && CheckSecondAccountAvailability)
                {
                    User? UserInfo = await _userManager.FindByIdAsync(Id2.ToString());
                    if(UserInfo != null)
                    {
                        await _signInManager.SignInAsync(UserInfo, true);
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> Enable2FAAsync(string? Email, string? Code)
        {
            if (Email != null && Code != null)
            {
                bool TryGetValue = _memoryCache.TryGetValue(Email + "_singleUseCode", out string? CacheCode);
                if (TryGetValue && Code.Equals(CacheCode))
                {
                    bool TryGetTheToken = _memoryCache.TryGetValue(Email + "_2FAtoken", out string? Token);
                    if (TryGetTheToken)
                    {
                        User? UserInfo = await _userManager.FindByEmailAsync(Email);
                        if (UserInfo != null && Token != null)
                        {
                            bool VerificationResult = await _userManager.VerifyTwoFactorTokenAsync(UserInfo, TokenOptions.DefaultProvider, Token);
                            if (VerificationResult)
                            {
                                IdentityResult? Result = await _userManager.SetTwoFactorEnabledAsync(UserInfo, true);
                                if (Result.Succeeded) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> Disable2FAAsync(string? Email, string? Code)
        {
            if(Email != null && Code != null)
            {
                bool TryGetValue = _memoryCache.TryGetValue(Email + "_singleUseCode", out string? CacheCode);
                if(TryGetValue && Code.Equals(CacheCode))
                {
                    User? UserInfo = await _userManager.FindByEmailAsync(Email);
                    if(UserInfo != null)
                    {
                        _memoryCache.TryGetValue(Email + "_singleUseToken", out string? Token);                 
                        if(Token != null)
                        {
                            bool ValidationResult = await _userManager.VerifyTwoFactorTokenAsync(UserInfo, TokenOptions.DefaultProvider, Token);
                            if(ValidationResult)
                            {
                                IdentityResult? Result =  await _userManager.SetTwoFactorEnabledAsync(UserInfo, false);
                                if(Result.Succeeded) return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
