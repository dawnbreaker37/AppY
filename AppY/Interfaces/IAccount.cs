using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IAccount : IBase<User>
    {
        public Task<(int, string?)?> SignUpAsync(SignUp SignUpModel);
        public Task<bool> SignInAsync(SignIn SignInModel);
        public Task<bool> UpdatePasswordAsync(UpdatePassword Model);
        public Task<bool> ResetPasswordAsync(ChangePassword Model);
        public Task<int> CheckAccountCredentialsAsync(string? UserName, string? Password);
        public IQueryable<LinkedAccount_ViewModel>? GetLinkedAccounts(int Id);
    }
}
