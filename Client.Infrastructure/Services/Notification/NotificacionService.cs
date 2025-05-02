using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Notifications.Query;
using IGift.Application.Responses.Notification;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using static IGift.Shared.Constants.AppConstants.Controller;

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
            var idUser = await _localStorage.GetItemAsync<string>(AppConstants.Local.IdUser);

            if (idUser == null)
                return await Result<IEnumerable<NotificationResponse>>.FailAsync("Error al obtener el usuario correspondiente");

            var query = new GetAllNotificationQuery
            {
                IdUser = idUser!
            };

            var url = AppConstants.Controller.Notification.GetAll;

            var response = await _httpClient.PostAsJsonAsync(url, query);
            var result = await response.ToResult<IEnumerable<NotificationResponse>>();
            return result;
        }
    }
}
