using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MultiservicioB.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _primaryOptions;
        private readonly SmtpFallbackOptions _fallbackOptions;

        public SmtpEmailSender(
            IOptions<SmtpOptions> primaryOptions,
            IOptions<SmtpFallbackOptions> fallbackOptions)
        {
            _primaryOptions = primaryOptions.Value;
            _fallbackOptions = fallbackOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = CreateMessage(email, subject, htmlMessage);

            try
            {
                await SendAsync(message, _primaryOptions);
            }
            catch (Exception exception) when (IsOperationalSmtpFailure(exception))
            {
                try
                {
                    await SendAsync(message, _fallbackOptions);
                }
                catch (Exception fallbackException)
                {
                    throw new InvalidOperationException(
                        "El correo no pudo enviarse por el servidor SMTP principal ni por el servidor secundario.",
                        fallbackException);
                }
            }
        }

        private static MimeMessage CreateMessage(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody();
            return message;
        }

        private static async Task SendAsync(MimeMessage message, SmtpOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Host) ||
                string.IsNullOrWhiteSpace(options.FromEmail))
            {
                throw new InvalidOperationException("Configure Host y FromEmail para enviar correos.");
            }

            message.From.Clear();
            message.From.Add(new MailboxAddress(options.FromName, options.FromEmail));

            using var client = new SmtpClient();
            await client.ConnectAsync(options.Host, options.Port, GetSecureSocketOptions(options));

            if (!string.IsNullOrWhiteSpace(options.UserName))
            {
                await client.AuthenticateAsync(options.UserName, options.Password);
            }

            if (!string.IsNullOrWhiteSpace(options.UserName) &&
                !string.Equals(options.UserName, options.FromEmail, StringComparison.OrdinalIgnoreCase))
            {
                await client.SendAsync(
                    message,
                    MailboxAddress.Parse(options.UserName),
                    message.To.Mailboxes);
            }
            else
            {
                await client.SendAsync(message);
            }

            await client.DisconnectAsync(true);
        }

        private static SecureSocketOptions GetSecureSocketOptions(SmtpOptions options)
        {
            if (!options.EnableSsl)
            {
                return SecureSocketOptions.None;
            }

            return options.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;
        }

        private static bool IsOperationalSmtpFailure(Exception exception)
        {
            return exception is SmtpCommandException or
                SmtpProtocolException or
                IOException or
                TimeoutException;
        }
    }
}
