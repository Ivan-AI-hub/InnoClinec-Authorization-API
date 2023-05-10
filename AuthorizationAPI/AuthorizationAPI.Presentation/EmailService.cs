using AuthorizationAPI.Presentation.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AuthorizationAPI.Presentation
{
    public class EmailService
    {
        private EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Sends message to specific email with specific message subject
        /// </summary>
        public async Task SendEmailAsync(string email, string subject, string message, CancellationToken cancellationToken = default)
        {
            using var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.EmailAddress));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(_emailSettings.EmailAddress, _emailSettings.Password, cancellationToken);
                await client.SendAsync(emailMessage, cancellationToken);

                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}
