using IGift.Application.Responses.Chat;

namespace Client.Infrastructure.Services.Notification
{
    public class NotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NotificationResponse>> GetAll()
        {
            var response= await _httpClient.GetAsync("asd");
            return null;
        }
    }
}
