using Azure.Core;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Configuration;
using System.Net;
using System.Net.Mail;

namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly SmtpClient _client;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _client = new SmtpClient
            {
                Host = _emailSettings.MailServer,
                Port = _emailSettings.MailPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password)
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                Subject = subject,
                SubjectEncoding = System.Text.Encoding.UTF8,
                Body = message,
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await _client.SendMailAsync(mailMessage);
        }
    }
}
