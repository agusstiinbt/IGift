using IGift.Application.Responses.Chat;
using IGift.Application.Responses.Identity.Users;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Notifications
{
    public interface INotification
    {
        Task<IResult<List<NotificationResponse>>> GetAllAsync();
    }
}
