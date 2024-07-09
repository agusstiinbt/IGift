using Client.Infrastructure.Extensions;
using IGift.Application.Responses.Chat;
using IGift.Shared;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<NotificationResponse>>> GetAll()
        {
            string idUser = "5fd28233-b939-4646-9178-8687357797fb";
            var response = await _httpClient.GetAsync(AppConstants.Controllers.NotificationController.GetAll+ idUser);
            return await response.ToResult<List<NotificationResponse>>();
        }
    }
}
