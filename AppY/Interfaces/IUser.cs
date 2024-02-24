using AppY.Models;
using AppY.Repositories;

namespace AppY.Interfaces
{
    public interface IUser : IBase<User>
    {
        public Task<bool> IsEmailUnique(string? Email);
        public Task<bool> IsUsernameUnique(string? Username);
    }
}
