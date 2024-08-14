using IGift.Application.Responses.Notification;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Notification
{
    public interface INotificationService
    {
        Task<IResult<IEnumerable<NotificationResponse>>> GetAll();
    }
}
