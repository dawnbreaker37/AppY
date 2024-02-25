using AppY.Data;
using AppY.Interfaces;
using AppY.Models;
using AppY.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AppY.Repositories
{
    public class Account : Base<User>, IAccount
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUser _userRepository;

        public Account(Context context, UserManager<User> userManager, SignInManager<User> signInManager, IUser userRepository) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }

        public Task<bool> LogOutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SignInAsync(SignIn SignInModel)
        {
            if(SignInModel != null && (!String.IsNullOrEmpty(SignInModel.Email) && !String.IsNullOrEmpty(SignInModel.Password)))
            {
                SignInResult? Result = null;
                if(SignInModel.IsViaUsername)
                {
                    Result = await _signInManager.PasswordSignInAsync(SignInModel.Email, SignInModel.Password, true, true);
                }
                else
                {
                    User? UserInfo = await _userManager.FindByEmailAsync(SignInModel.Email);
                    if (UserInfo != null) Result = await _signInManager.PasswordSignInAsync(UserInfo, SignInModel.Password, true, true);
                }

                if (Result != null && Result.Succeeded) return true;
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
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
