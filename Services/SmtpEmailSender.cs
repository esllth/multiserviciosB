using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MultiservicioB.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;

        public SmtpEmailSender(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(_options.Host) ||
                string.IsNullOrWhiteSpace(_options.FromEmail))
            {
                throw new InvalidOperationException("Configure Smtp:Host y Smtp:FromEmail para enviar correos.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody();

            using var client = new SmtpClient();
            var secureSocketOptions = GetSecureSocketOptions();

            await client.ConnectAsync(_options.Host, _options.Port, secureSocketOptions);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password);
            }

            if (!string.IsNullOrWhiteSpace(_options.UserName) &&
                !string.Equals(_options.UserName, _options.FromEmail, StringComparison.OrdinalIgnoreCase))
            {
                await client.SendAsync(
                    message,
                    MailboxAddress.Parse(_options.UserName),
                    message.To.Mailboxes);
            }
            else
            {
                await client.SendAsync(message);
            }

            await client.DisconnectAsync(true);
        }

        private SecureSocketOptions GetSecureSocketOptions()
        {
            if (!_options.EnableSsl)
            {
                return SecureSocketOptions.None;
            }

            return _options.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;
        }
    }
}
