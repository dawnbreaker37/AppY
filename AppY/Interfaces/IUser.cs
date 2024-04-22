using AppY.Models;
using AppY.Repositories;
using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IUser : IBase<User>
    {
        public Task<User?> GetMainUserInfoAsync(int Id);
        public Task<User?> GetAverageUserInfoAsync(string? Shortname);
        public Task<User?> GetAverageUserInfoAsync(int Id);
        public Task<User?> GetUserSuperShortInfoAsync(int Id);
        public Task<User?> GetUserSuperShortInfoEvenIfPrivateAsync(int Id);
        public Task<User?> GetUserSuperShortInfoAsync(string? Shortname);
        public Task<int> GetCurrentUserAccessLevelAsync(int Id, int UserId);
        public Task<bool> EditUserInfoAsync(EditUserInfo_ViewModel Model);
        public Task<bool> EditPrivacySettingsAsync(EditUserPrivacySettings Model);
        public Task<bool> EditAvatarDesignAsync(EditAvatarColors Model);
        public Task<string?> SetProfilePhotoAsync(int Id, IFormFile File);
        public Task<bool> DeleteProfilePhotoAsync(int Id, string? File);
        public Task<int> SetLastSeenAsync(int Id); 
        public Task<bool> IsEmailUniqueAsync(string? Email);
        public Task<bool> IsUsernameUniqueAsync(string? Username);
        public Task<bool> IsShortnameUniqueAsync(int ExceptId, string? Shortname);
        public Task<string?> GetReserveCodeViaEmailAsync(string? Email);
        public Task<string?> SubmitSingleUseCodeAsync(string? Email, string Code);
        public Task<string?> SubmitReserveCodeViaEmailAsync(string? Email, string Code);
        public Task<bool> SubmitReserveCodeAsync(int Id, string? Code);
        public Task<bool> SubmitReserveCodeAsync(string UsernameOrEmail, string? Code);
        public IQueryable<User>? FindUsers(string? Keyword);
        public string? UnpicturedAvatarSelector(User? UserInfo);
        public string? SetLastSeenText(DateTime? LastSeen);
        public string? AutodeleteDelay(double MinsValue);
    }
}
