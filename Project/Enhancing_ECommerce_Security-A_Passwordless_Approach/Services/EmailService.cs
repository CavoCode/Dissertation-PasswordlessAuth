using Microsoft.Extensions.Options;
using MimeKit;
using NuGet.Configuration;

namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["Sender"]));
            mailMessage.To.Add(new MailboxAddress("User", email));
            mailMessage.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = message };
            mailMessage.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(emailSettings["MailServer"], int.Parse(emailSettings["MailPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailSettings["Sender"], emailSettings["Password"]);
                await smtp.SendAsync(mailMessage);
            }
            finally
            {
                smtp.Disconnect(true);
            }
        }
    }
}
