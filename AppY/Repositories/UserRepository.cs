using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AppY.Repositories
{
    public class UserRepository : Base<User>, IUser
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IMemoryCache _memoryCache;
        public UserRepository(Context context, UserManager<User> userManager, IMemoryCache memoryCache) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        public async Task<string?> GetReserveCodeViaEmailAsync(string? Email)
        {
            if (!String.IsNullOrEmpty(Email)) return await _context.Users.AsNoTracking().Where(u => u.Email == Email && !u.IsDisabled).Select(u => u.ReserveCode).FirstOrDefaultAsync();
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

        public async Task<string?> SubmitReserveCodeViaEmailAsync(string? Email, string ReserveCode)
        {
            if(!String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(ReserveCode))
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
            if(!String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(Code))
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
    }
}
