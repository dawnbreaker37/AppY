using AppY.Data;
using AppY.Interfaces;
using AppY.ViewModels;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;

namespace AppY.Repositories
{
    public class MailMessages : IMailMessages
    {
        public async Task<bool> SendMessageAsync(MailKitModel MailKitModel, SendEmail Model)
        {














            //if(MailKitModel != null && Model != null)
            //{
            //    using(MimeMessage mimeMessage = new MimeMessage())
            //    {
            //        mimeMessage.From.Add(new MailboxAddress(Model.Title, Model.SentFrom));
            //        mimeMessage.To.Add(new MailboxAddress("", Model.SendTo));
            //        mimeMessage.Subject = Model.Subject;
            //        mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //        {
            //            Text = Model.Body,                       
            //        };
            //        mimeMessage.Date = DateTime.Now;
            //        using(SmtpClient smtpClient = new SmtpClient())
            //        {
            //            await smtpClient.ConnectAsync(MailKitModel.Host, MailKitModel.Port);
            //            await smtpClient.AuthenticateAsync(MailKitModel.EmailAddress, MailKitModel.Password);
            //            await smtpClient.SendAsync(mimeMessage);
            //            await smtpClient.DisconnectAsync(true);

            //            return true;
            //        }
            //    }
            //}
            return false;
        }
    }
}
