using IGift.Application.Requests.Identity.Email;

namespace IGift.Application.Interfaces.IMailService
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
