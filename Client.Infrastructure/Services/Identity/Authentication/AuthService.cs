using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Identity.Token;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Responses.Identity.Users;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using static IGift.Shared.Constants.AppConstants.Controllers;
namespace Client.Infrastructure.Services.Identity.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _js;
        private readonly ISnackbar _snackBar;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage, IJSRuntime js, ISnackbar snackBar)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _js = js;
            _snackBar = snackBar;
        }

        public async Task<IResult> Register(UserCreateRequest registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Controllers.Users.Register, registerModel);
            return await response.ToResult<IResult>();
        }

        public async Task<IResult> Login(UserLoginRequest loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Controllers.TokenController.LogIn, loginModel);
            var result = await response.ToResult<UserLoginResponse>();

            if (result.Succeeded)
            {
                //separamos las propiedades
                var token = result!.Data.Token;
                var refreshToken = result!.Data.RefreshToken;
                var idUser = result!.Data.IdUser;

                //gurdamos cada propiedad en el cliente
                await _localStorage.SetItemAsync(AppConstants.Local.AuthToken, token);
                await _localStorage.SetItemAsync(AppConstants.Local.RefreshToken, refreshToken);
                await _localStorage.SetItemAsync(AppConstants.Local.IdUser, idUser);


                //preparamos los headers con el token correcto
                await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();

                // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                return await Result.SuccessAsync();
            }

            return result;
        }

        public async Task<IResult> Logout()
        {
            //Usamos entre paréntesis porque el método MarkUserAsLoggedOut es propio de IGIft...provider
            await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();

            return await Result.SuccessAsync();
        }

        public async Task Disconnect<T>(DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            await _js.InitializeInactivityTimer(dotNetObjectReference);
        }

        public async Task<string> TryRefreshToken()
        {
            var tokenDisponible = await _localStorage.GetItemAsync<string>(AppConstants.Local.RefreshToken);
            if (string.IsNullOrEmpty(tokenDisponible)) return string.Empty;

            //var authState = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync(); es lo mismo
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var expiration = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expiration));
            var timeUTC = DateTime.UtcNow;

            var diff = expirationTime - timeUTC;
            if (diff.TotalMinutes <= 1)
            {
                return await RefreshToken();
            }

            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken() => await RefreshToken();

        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(AppConstants.Local.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(AppConstants.Local.RefreshToken);

            var response = await _httpClient.PostAsJsonAsync(TokenController.RefreshToken, new TokenRequest { Token = token!, RefreshToken = refreshToken! });
            var result = await response.ToResult<UserLoginResponse>();

            if (!result.Succeeded)
            {
                throw new Exception("error al refrescar el token");
            }

            token = result.Data.Token;
            refreshToken = result.Data.RefreshToken;

            await _localStorage.SetItemAsync(AppConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(AppConstants.Local.RefreshToken, refreshToken);


            await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();

            return token;
        }
    }
}
