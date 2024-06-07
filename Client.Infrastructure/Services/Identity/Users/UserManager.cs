using Client.Infrastructure.Extensions;
using IGift.Application.Responses;
using IGift.Application.Responses.Identity.Users;
using IGift.Shared;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;
using System.Net.Http;
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

        public async Task<IResult> GetUsersAsync()
        {
            //TODO el applicationUSerResponse debe cargar la lista de giftcards ordenadas primero por activas y fecha de creación
            var response = await _http.GetAsync(AppConstants.Controllers.Users.GetAll);
            return await response.ToResult<List<UserResponse>>();
        }
    }
}
