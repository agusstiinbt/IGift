using IGift.Application.Responses.Chat;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Notification
{
    public interface INotificationService
    {
        Task<IResult<List<NotificationResponse>>> GetAll();
    }
}
