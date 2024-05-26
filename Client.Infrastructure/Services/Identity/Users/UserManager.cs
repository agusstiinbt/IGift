using IGift.Application.Responses;
using IGift.Shared;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Client.Infrastructure.Services.Identity.Users
{
    public class UserManager : IUserManager
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public UserManager(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<IResult<List<ApplicationUserResponse>>> GetUsersAsync()
        {
            //TODO el applicationUSerResponse debe cargar la lista de giftcards ordenadas primero por activas y fecha de creación
            var response = await _http.GetAsync(AppConstants.Users.GetAll);
            var result = await response.Content.ReadFromJsonAsync<Result<List<ApplicationUserResponse>>>();
            return result!;
        }
    }
}
