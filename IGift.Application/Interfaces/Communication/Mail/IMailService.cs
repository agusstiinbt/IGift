using IGift.Application.CQRS.Identity.Email;

namespace IGift.Application.Interfaces.Communication.Mail
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
