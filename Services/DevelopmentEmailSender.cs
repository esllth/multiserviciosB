using Microsoft.AspNetCore.Identity.UI.Services;

namespace MultiservicioB.Services
{
    public class DevelopmentEmailSender : IEmailSender
    {
        private readonly ILogger<DevelopmentEmailSender> _logger;

        public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Email para {Email}. Asunto: {Subject}. Mensaje: {Message}", email, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
