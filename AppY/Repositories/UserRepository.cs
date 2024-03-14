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
            if (Id != 0) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, IsDisabled = u.IsDisabled, AvatarUrl = u.AvatarUrl, Email = u.Email, AvatarBgColor = u.AvatarBgColor, AvatarFgColor = u.AvatarFgColor, EmailConfirmed = u.EmailConfirmed, Description = u.Description, PseudoName = u.PseudoName, ShortName = u.ShortName, PasswordChanged = u.PasswordChanged, AvatarStickerUrl = u.AvatarStickerUrl }).FirstOrDefaultAsync(u => u.Id == Id && !u.IsDisabled);
            else return null;
        }

        public async Task<User?> GetAverageUserInfoAsync(string? Shortname)
        {
            if (!String.IsNullOrWhiteSpace(Shortname)) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, IsDisabled = u.IsDisabled, ShortName = u.ShortName, AvatarUrl = u.AvatarUrl, PseudoName = u.PseudoName, CreatedAt = u.CreatedAt }).FirstOrDefaultAsync(u => u.ShortName != null && (u.ShortName.ToLower() == Shortname.ToLower()));
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
            if (Id != 0) return await _context.Users.AsNoTracking().Select(u => new User { Id = u.Id, PseudoName = u.PseudoName, ShortName = u.ShortName }).FirstOrDefaultAsync(u => u.Id == Id);
            else return null;
        }

        public async Task<User?> GetUserSuperShortInfoAsync(string? Shortname)
        {
            if (!String.IsNullOrWhiteSpace(Shortname)) return await _context.Users.AsNoTracking().Select(u => new User { PseudoName = u.PseudoName, ShortName = u.ShortName }).FirstOrDefaultAsync(u => u.ShortName.ToLower() == Shortname.ToLower());
            else return null;
        }
    }
}
