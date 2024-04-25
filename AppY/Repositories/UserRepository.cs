using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AppY.Repositories
{
    public class UserRepository : Base<User>, IUser
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemoryCache _memoryCache;
        public UserRepository(Context context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment, IMemoryCache memoryCache) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _memoryCache = memoryCache;
        }

        public async Task<User?> GetMainUserInfoAsync(int Id)
        {
            if (Id != 0) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, IsDisabled = u.IsDisabled, AvatarUrl = u.AvatarUrl, Email = u.Email, AvatarBgColor = u.AvatarBgColor, AvatarFgColor = u.AvatarFgColor, EmailConfirmed = u.EmailConfirmed, Description = u.Description, EcoModeOnAt = u.EcoModeOnAt, PseudoName = u.PseudoName, ShortName = u.ShortName, PasswordChanged = u.PasswordChanged, AvatarStickerUrl = u.AvatarStickerUrl, IsPrivate = u.IsPrivate, AreMessagesAutoDeletable = u.AreMessagesAutoDeletable }).FirstOrDefaultAsync(u => u.Id == Id && !u.IsDisabled);
            else return null;
        }

        public async Task<User?> GetAverageUserInfoAsync(string? Shortname)
        {
            if (!String.IsNullOrWhiteSpace(Shortname)) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, IsDisabled = u.IsDisabled, ShortName = u.ShortName, AvatarUrl = u.AvatarUrl, PseudoName = u.PseudoName, CreatedAt = u.CreatedAt, LastSeen = u.LastSeen }).FirstOrDefaultAsync(u => u.ShortName != null && (u.ShortName.ToLower() == Shortname.ToLower()));
            else return null;
        }

        public async Task<User?> GetAverageUserInfoAsync(int Id)
        {
            if (Id > 0) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, IsDisabled = u.IsDisabled, ShortName = u.ShortName, LastSeen = u.LastSeen, PseudoName = u.PseudoName, CreatedAt = u.CreatedAt, Description = u.Description, AvatarUrl = u.AvatarUrl, AvatarStickerUrl = u.AvatarUrl == null ? u.AvatarStickerUrl : null, AvatarBgColor = u.AvatarUrl == null ? u.AvatarBgColor : null, AvatarFgColor = u.AvatarUrl == null ? u.AvatarFgColor : null }).FirstOrDefaultAsync(u => u.Id == Id && !u.IsDisabled);
            else return null;
        }

        public async Task<string?> GetReserveCodeViaEmailAsync(string? Email)
        {
            if (!String.IsNullOrWhiteSpace(Email)) return await _context.Users.AsNoTracking().Where(u => u.Email == Email && !u.IsDisabled).Select(u => u.ReserveCode).FirstOrDefaultAsync();
            else return null;
        }

        public async Task<bool> IsEmailUniqueAsync(string? Email)
        {
            if (Email != null) return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == Email);
            else return false;
        }

        public async Task<bool> IsUsernameUniqueAsync(string? Username)
        {
            if (Username != null) return await _context.Users.AsNoTracking().AnyAsync(u => u.UserName == Username);
            else return false;
        }

        public async Task<bool> IsShortnameUniqueAsync(int ExceptId, string? Shortname)
        {
            if (!String.IsNullOrEmpty(Shortname) && ExceptId != 0)
            {
                bool Result = await _context.Users.AsNoTracking().AnyAsync(u => u.Id != ExceptId && u.ShortName != null && u.ShortName.ToLower() == Shortname.ToLower());
                if (!Result) return true;
            }
            return false;
        }

        public async Task<string?> SubmitReserveCodeViaEmailAsync(string? Email, string ReserveCode)
        {
            if(!String.IsNullOrWhiteSpace(Email) && !String.IsNullOrEmpty(ReserveCode))
            {
                User? UserInfo = await _userManager.FindByEmailAsync(Email);
                if(UserInfo != null && UserInfo.ReserveCode == ReserveCode)
                {
                    return await _userManager.GeneratePasswordResetTokenAsync(UserInfo);
                }
            }
            return null;
        }

        public async Task<bool> SubmitReserveCodeAsync(int Id, string? Code)
        {
            if(Id > 0 && !String.IsNullOrWhiteSpace(Code))
            {
                return await _context.Users.AsNoTracking().AnyAsync(u => u.Id == Id && u.ReserveCode == Code);
            }
            return false;
        }

        public async Task<bool> SubmitReserveCodeAsync(string? UsernameOrEmail, string? Code)
        {
            if (!String.IsNullOrWhiteSpace(UsernameOrEmail) && !String.IsNullOrWhiteSpace(Code))
            {
                return await _context.Users.AsNoTracking().AnyAsync(u => ((u.Email != null && u.Email == UsernameOrEmail) || (u.UserName != null && u.UserName == UsernameOrEmail)) && u.ReserveCode == Code);
            }
            else return false;
        }

        public async Task<string?> SubmitSingleUseCodeAsync(string? Email, string Code)
        {
            if(!String.IsNullOrWhiteSpace(Email) && !String.IsNullOrWhiteSpace(Code))
            {
                bool IsCodeTrue = _memoryCache.TryGetValue(Email + "_singleUseCode", out string? RealCode);
                if(IsCodeTrue && RealCode == Code)
                {
                    User? UserInfo = await _userManager.FindByEmailAsync(Email);
                    if (UserInfo != null) return await _userManager.GeneratePasswordResetTokenAsync(UserInfo);
                }
            }
            return null;
        }

        public async Task<bool> EditUserInfoAsync(EditUserInfo_ViewModel Model)
        {
            if(!String.IsNullOrWhiteSpace(Model.PseudoName) && !String.IsNullOrWhiteSpace(Model.ShortName) && Model.Id != 0)
            {
                int Result = 0;
                bool IsShortnameUnique = await IsShortnameUniqueAsync(Model.Id, Model.ShortName);
                if(IsShortnameUnique) Result = await _context.Users.AsNoTracking().Where(u => u.Id == Model.Id && !u.IsDisabled).ExecuteUpdateAsync(u => u.SetProperty(u => u.PseudoName, Model.PseudoName).SetProperty(u => u.Description, Model.Description).SetProperty(u => u.ShortName, Model.ShortName.ToLower()));
                else Result = await _context.Users.AsNoTracking().Where(u => u.Id == Model.Id && !u.IsDisabled).ExecuteUpdateAsync(u => u.SetProperty(u => u.PseudoName, Model.PseudoName).SetProperty(u => u.Description, Model.Description));
                
                if (Result != 0) return true;
            }
            return false;
        }

        public async Task<bool> EditAvatarDesignAsync(EditAvatarColors Model)
        {
            if(Model.Id != 0)
            {
                int Result = await _context.Users.AsNoTracking().Where(u => u.Id == Model.Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.AvatarBgColor, Model.BgColor).SetProperty(u => u.AvatarFgColor, Model.FgColor).SetProperty(p => p.AvatarStickerUrl, Model.AvatarStickerUrl));
                if (Result != 0) return true;
            }
            return false;
        }

        public string? UnpicturedAvatarSelector(User? UserInfo)
        {
            if (UserInfo != null)
            {
                if (UserInfo.AvatarStickerUrl == null) return UserInfo.PseudoName != null ? UserInfo.PseudoName[0].ToString() : "?";
                else return UserInfo.AvatarStickerUrl;
            }
            else return "?";
        }

        public async Task<string?> SetProfilePhotoAsync(int Id, IFormFile File)
        {
            if(Id != 0 && File != null)
            {
                string? FileExtension = Path.GetExtension(File.FileName);
                string? NewFileName = Guid.NewGuid().ToString("D").Substring(4, 16);
                using(FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "/Avatars/" + NewFileName + FileExtension, FileMode.Create))
                {
                    await File.CopyToAsync(fs);
                    int Result = await _context.Users.AsNoTracking().Where(u => u.Id == Id && !u.IsDisabled).ExecuteUpdateAsync(u => u.SetProperty(u => u.AvatarUrl, NewFileName + FileExtension));
                    if (Result > 0) return NewFileName + FileExtension;
                }
            }
            return null;
        }

        public async Task<bool> DeleteProfilePhotoAsync(int Id, string? File)
        {
            if(Id != 0)
            {
                int Result = await _context.Users.AsNoTracking().Where(u => u.Id == Id && !u.IsDisabled).ExecuteUpdateAsync(u => u.SetProperty(u => u.AvatarUrl, File));
                if (Result != 0) return true;
            }
            return false;
        }

        public async Task<User?> GetUserSuperShortInfoAsync(int Id)
        {
            if (Id != 0) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, AvatarUrl = u.AvatarUrl, PseudoName = u.PseudoName, ShortName = u.ShortName, IsPrivate = u.IsPrivate, LastSeen = u.LastSeen }).FirstOrDefaultAsync(u => u.Id == Id && !u.IsPrivate);
            else return null;
        }

        public async Task<User?> GetUserSuperShortInfoEvenIfPrivateAsync(int Id)
        {
            if (Id != 0) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, AvatarUrl = u.AvatarUrl, PseudoName = u.PseudoName, LastSeen = u.LastSeen, ShortName = u.ShortName, IsPrivate = u.IsPrivate }).FirstOrDefaultAsync(u => u.Id == Id);
            else return null;
        }

        public async Task<User?> GetUserSuperShortInfoAsync(string? Shortname)
        {
            return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, PseudoName = u.PseudoName, ShortName = u.ShortName, IsPrivate = u.IsPrivate, LastSeen = u.LastSeen }).FirstOrDefaultAsync(u => u.ShortName != null && u.ShortName.ToLower().Equals(Shortname!.ToLower()) && !u.IsPrivate);
        }

        public IQueryable<User>? FindUsers(string? Keyword)
        {
            if (!String.IsNullOrWhiteSpace(Keyword))
            {
                return _context.Users.AsNoTracking().Where(u => !u.IsPrivate && !u.IsDisabled && (u.UserName != null && u.UserName.ToLower().Contains(Keyword.ToLower()) || u.ShortName != null && u.ShortName.ToLower().Contains(Keyword.ToLower()) || u.PseudoName != null && u.PseudoName.ToLower().Contains(Keyword.ToLower()))).Select(u => new User { Id = u.Id, PseudoName = u.PseudoName, ShortName = u.ShortName, IsPrivate = u.IsPrivate });
            }
            else return null;
        }

        public async Task<int> GetCurrentUserAccessLevelAsync(int Id, int UserId)
        {
            if (Id != 0) return await _context.DiscussionUsers.Where(d => d.UserId == UserId && d.DiscussionId == Id && !d.IsDeleted).Select(d => d.AccessLevel).FirstOrDefaultAsync();
            else return 0;
        }

        public async Task<bool> EditPrivacySettingsAsync(EditUserPrivacySettings Model)
        {
            if (Model.Id > 0 && Model.AreMessagesAutoDeletable >= 0)
            {
                int Result = await _context.Users.AsNoTracking().Where(d => d.Id == Model.Id && !d.IsDisabled).ExecuteUpdateAsync(d => d.SetProperty(d => d.IsPrivate, Model.IsPrivate).SetProperty(d => d.AreMessagesAutoDeletable, Model.AreMessagesAutoDeletable));
                if (Result > 0) return true;
            }
            return false;
        }

        public string? AutodeleteDelay(double MinsValue)
        {
            if (MinsValue >= 60 && MinsValue < 1440) return Math.Round(MinsValue / 60, 0) + " hour(s)";
            else if (MinsValue >= 1440 && MinsValue < 10080) return Math.Round((MinsValue / 60) / 24, 0) + " day(s)";
            else if (MinsValue >= 10080 && MinsValue < 43200) return Math.Round((MinsValue / 60) / 24, 0) + " week(s)";
            else if (MinsValue >= 43200) return Math.Round((MinsValue / 60) / 24, 0) + " month(s)";
            else return Math.Round(MinsValue, 0) + " min(s)";
        }

        public string? SetLastSeenText(DateTime? LastSeen)
        {
            DateTime Current = DateTime.Now;
            if (LastSeen.HasValue)
            {
                double DaysBetween = Current.Subtract(LastSeen.Value).TotalDays;

                if (DaysBetween >= 370) return "last seen over a year ago";
                else if (DaysBetween >= 31 && DaysBetween < 370) return "last seen over a month ago";
                else if (DaysBetween > 7 && DaysBetween < 31) return "last seen within a month";
                else if (DaysBetween > 1 && DaysBetween < 7) return "last seen " + Math.Round(DaysBetween, 0) + " days ago, at " + LastSeen.Value.ToShortTimeString();
                else if (DaysBetween == 1) return "last seen yesterday, at " + LastSeen.Value.ToShortTimeString();
                else return "last seen today, at " + LastSeen.Value.ToShortTimeString();
            }
            else return "last seen recently";
        }

        public async Task<int> SetLastSeenAsync(int Id)
        {
            if (Id > 0)
            {
                int Result = await _context.Users.AsNoTracking().Where(u => u.Id == Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.LastSeen, DateTime.Now));
                if (Result > 0) return Id;
            }
            return 0;
        }

        public async Task<int> EditEcoModeSettings(int Id, int BatteryLevel)
        {
            if(Id > 0)
            {
                BatteryLevel = BatteryLevel >= 0 && BatteryLevel <= 45 ? BatteryLevel : 20;
                await _context.Users.AsNoTracking().Where(u => u.Id == Id).ExecuteUpdateAsync(u => u.SetProperty(u => u.EcoModeOnAt, BatteryLevel));
                return BatteryLevel;
            }
            return 0;
        }
    }
}
