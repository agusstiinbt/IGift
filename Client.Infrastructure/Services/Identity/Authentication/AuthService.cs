using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IGift.Shared.Operations.Login;
using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using IGift.Application.Requests.Identity;
using Client.Infrastructure.Extensions;
using IGift.Shared;
using Microsoft.JSInterop;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage,
                           IJSRuntime js)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _js = js;
        }

        public async Task<IResult> Register(ApplicationUserRequest registerModel)
        {
            //using var content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsJsonAsync(Endpoints.Users.Register, registerModel);
            //var result = await response.Content.ReadFromJsonAsync<IResult>();
            //return result!;
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Users.Register, registerModel);
            return await response.ToResult();
        }

        public async Task<IResult> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Users.LogIn, loginModel);
            var result = await response.ToResult<TokenResponse>();

            if (result.Succeeded)
            {
                var token = result!.Data.Token;
                await _localStorage.SetItemAsync("authToken", token);
                ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggin(token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Data.Token);
                return await Result.SuccessAsync();
            }

            return result;
        }

        public async Task<IResult> Logout()
        {
            await _js.RemoverDelLocalStorage(AppConstants.StorageConstants.Local.AuthToken);
            await _js.RemoverDelLocalStorage(AppConstants.StorageConstants.Local.RefreshToken);
            await _js.RemoverDelLocalStorage(AppConstants.StorageConstants.Local.UserImageURL);
            
            //Usamos entre paréntesis porque el método MarkUserAsLoggedOut es propio de IGIft...provider
            ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return await Result.SuccessAsync();
        }

        public async Task Disconnect<T>(DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            await _js.InitializeInactivityTimer<T>(dotNetObjectReference);
        }
    }
}
