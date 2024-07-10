using Client.Infrastructure.Extensions;
using IGift.Application.Features.Notifications.Query;
using IGift.Application.Responses.Chat;
using IGift.Shared;
using IGift.Shared.Wrapper;
using System.Net.Http.Json;

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
            //TODO implementar lógica para obtener el id de otra manera
            string idUser = "5fd28233-b939-4646-9178-8687357797fb";

            var query = new GetAllNotificationQuery(idUser);

            var url = AppConstants.Controllers.NotificationController.GetAll;

            var response = await _httpClient.PostAsJsonAsync(url, query);
            var result = await response.ToResult<List<NotificationResponse>>();
            return result;
        }
    }
}
