using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Notifications.Query;
using IGift.Application.Responses.Notification;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;

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

            var query = new GetAllNotificationQuery(idUser!);

            var url = ConstNotificationController.GetAll;

            var response = await _httpClient.PostAsJsonAsync(url, query);
            var result = await response.ToResult<IEnumerable<NotificationResponse>>();
            return result;
        }
    }
}
