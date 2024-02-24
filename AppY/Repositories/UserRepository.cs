using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using Microsoft.EntityFrameworkCore;

namespace AppY.Repositories
{
    public class UserRepository : Base<User>, IUser
    {
        private readonly Context _context;
        public UserRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailUnique(string? Email)
        {
            if (Email != null) return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == Email);
            else return false;
        }

        public async Task<bool> IsUsernameUnique(string? Username)
        {
            if (Username != null) return await _context.Users.AsNoTracking().AnyAsync(u => u.UserName == Username);
            else return false;
        }
    }
}
