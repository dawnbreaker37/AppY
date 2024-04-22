using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IAccount : IBase<User>
    {
        public Task<bool> EasyEntryAsync(int Id1, int Id2);
        public Task<(int, string?)?> SignUpAsync(SignUp SignUpModel);
        public Task<bool> SignInAsync(SignIn SignInModel);
        public Task<bool> UpdatePasswordAsync(UpdatePassword Model);
        public Task<bool> ResetPasswordAsync(ChangePassword Model);
        public Task<int> CheckAccountCredentialsAsync(string? UserName, string? Password);
        public Task<User?> LinkAccountsAsync(int Id1, int Id2, string? Codename);
        public Task<int> UnlinkAccountsAsync(int Id1, int Id2);
        public IQueryable<LinkedAccount_ViewModel>? GetLinkedAccounts(int Id);
    }
}
