using AppY.Models;
using AppY.Repositories;

namespace AppY.Interfaces
{
    public interface IUser : IBase<User>
    {
        public Task<bool> IsEmailUniqueAsync(string? Email);
        public Task<bool> IsUsernameUniqueAsync(string? Username);
        public Task<string?> GetReserveCodeViaEmailAsync(string? Email);
        public Task<string?> SubmitSingleUseCodeAsync(string? Email, string Code);
        public Task<string?> SubmitReserveCodeViaEmailAsync(string? Email, string Code);
    }
}
