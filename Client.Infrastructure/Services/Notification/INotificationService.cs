using IGift.Application.Responses.Chat;

namespace Client.Infrastructure.Services.Notification
{
    public interface INotificationService
    {
        Task<List<NotificationResponse>> GetAll();
    }
}
