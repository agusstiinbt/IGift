using IGift.Application.CQRS.Identity.Email;

namespace IGift.Application.Interfaces.IMailService
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
