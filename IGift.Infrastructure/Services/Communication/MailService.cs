using IGift.Application.CQRS.Identity.Email;
using IGift.Application.Interfaces.Communication.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IGift.Infrastructure.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly MailConfiguration _config;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailConfiguration> config, ILogger<MailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        //TODO implementar todos los ILoggers en toda la solución
        //TODO documentar qué hace cada línea de código
        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = new MailboxAddress(_config.DisplayName, request.From ?? _config.From),
                    Subject = request.Subject,
                    Body = new BodyBuilder
                    {
                        HtmlBody = request.Body
                    }.ToMessageBody()
                };

                email.To.Add(MailboxAddress.Parse(request.To));
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.UserName, _config.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.UserName, _config.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

            }
            catch (Exception e)
            {
                _logger.LogError("Servicio de correo: " + e.Message, e);
            }
        }
    }
}
