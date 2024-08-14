using Blazored.LocalStorage;
using Client.Infrastructure.Extensions;
using IGift.Application.Requests.Notifications.Query;
using IGift.Application.Responses.Notification;
using IGift.Shared;
using IGift.Shared.Wrapper;
using System.Net.Http.Json;

namespace Client.Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;


        public NotificationService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<IResult<IEnumerable<NotificationResponse>>> GetAll()
        {
            //TODO implementar lógica para obtener el id de otra manera?
            var idUser = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.IdUser);

            var query = new GetAllNotificationQuery(idUser);

            var url = AppConstants.Controllers.NotificationController.GetAll;

            var response = await _httpClient.PostAsJsonAsync(url, query);
            var result = await response.ToResult<IEnumerable<NotificationResponse>>();
            return result;
        }

    }
}
