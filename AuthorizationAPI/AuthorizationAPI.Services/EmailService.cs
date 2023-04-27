using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AuthorizationAPI.Services
{
    public class EmailService
    {
        private const string _fromName = "Лучшая больница";

        private const string _address = "InnoClinic@gmail.com";
        private const string _password = "1234";

        private const string _host = "smtp.gmail.com";
        private const int _port = 587;

        /// <summary>
        /// Sends message to specific email with specific message subject
        /// </summary>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_fromName, _address));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_address, _password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
