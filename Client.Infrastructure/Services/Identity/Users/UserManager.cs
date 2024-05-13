using Client.Infrastructure.Routes;
using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using System.Net.Http.Json;

namespace Client.Infrastructure.Services.Identity.Users
{
    public class UserManager : IUserManager
    {
        private readonly HttpClient _http;

        public UserManager(HttpClient http)
        {
            _http = http;
        }

        public async Task<IResult<List<ApplicationUserResponse>>> GetUsersAsync()
        {
            var response = await _http.GetAsync(UsersEndpoints.GetAll);
            var result = await response.Content.ReadFromJsonAsync<Result<List<ApplicationUserResponse>>>();
            return result!;
        }
    }
}
