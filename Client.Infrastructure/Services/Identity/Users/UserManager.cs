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

        public async Task<IResult> GetUsersAsync()
        {
            //TODO el applicationUSerResponse debe cargar la lista de giftcards ordenadas primero por activas y fecha de creación
            try
            {
                var response = await _http.GetAsync(AppConstants.Endpoints.UsersController.GetAll);
                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadFromJsonAsync<Result<List<LoginResponse>>>();
                }
                return await Result.FailAsync();
            }
            catch (Exception e)
            {

                throw;
            }
            return null; 
        }
    }
}
