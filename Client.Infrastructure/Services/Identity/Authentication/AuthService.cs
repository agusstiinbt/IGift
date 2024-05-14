using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using IGift.Shared.Operations.Register;
using IGift.Shared.Operations.Login;
using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using Client.Infrastructure.Routes;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/accounts", registerModel);
            if (!result.IsSuccessStatusCode)
                return new RegisterResult { Successful = true, Errors = null };
            return new RegisterResult { Successful = false, Errors = new List<string> { "Error occured" } };
        }

        public async Task<IResult> Login(LoginModel loginModel)
        {
            using var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(Endpoints.Login.LogIn, content);
            var result = await response.Content.ReadFromJsonAsync<Result<TokenResponse>>();

            if (result!.Succeeded)
            {
                var token = result!.Data.Token;
                await _localStorage.SetItemAsync("authToken", token);
                ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggin(token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Data.Token);
                return await Result.SuccessAsync("Autenticación correcta");
            }

            return await Result.FailAsync("Error en la autenticación.");
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            //Usamos entre paréntesis porque el método MarkUserAsLoggedOut es propio de IGIft...provider
            ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
