using AppY.ViewModels;

namespace AppY.Interfaces
{
    public interface IMailMessages
    {
        public Task<bool> SendMessageAsync(MailKitModel MailKitModel, SendEmail Model); 
    }
}
