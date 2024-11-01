using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender", _configuration["EmailSettings:FromAddress"]));
            message.To.Add(new MailboxAddress("Recipient", emailMessage.To));
            message.Subject = emailMessage.Subject;
            message.Body = new TextPart("html") { Text = emailMessage.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
