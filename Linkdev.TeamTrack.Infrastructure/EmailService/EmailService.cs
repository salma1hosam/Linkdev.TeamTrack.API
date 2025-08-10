using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Linkdev.TeamTrack.Infrastructure.EmailService
{
    public class EmailService(IOptions<SmtpConfiguration> _options) : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(MailboxAddress.Parse(_options.Value.SmtpEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = messageBody;
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_options.Value.SmtpServer, _options.Value.SmtpPort, SecureSocketOptions.StartTls);
                if (!string.IsNullOrEmpty(_options.Value.SmtpPassword))
                    await client.AuthenticateAsync(_options.Value.SmtpEmail, _options.Value.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
