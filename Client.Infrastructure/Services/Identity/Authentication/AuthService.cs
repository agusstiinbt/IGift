using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Client.Infrastructure.Extensions;
using IGift.Application.CQRS.Identity.Token;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Responses.Identity.Users;
using IGift.Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
namespace Client.Infrastructure.Services.Identity.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly ISnackbar _snackBar;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage,  ISnackbar snackBar)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            _snackBar = snackBar;
        }

        public async Task<IResult> Register(UserCreateRequest registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync(ConstUsersController.Register, registerModel);
            return await response.ToResult<IResult>();
        }

        public async Task<IResult> Login(UserLoginRequest loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(ConstTokenController.LogIn, loginModel);
            var result = await response.ToResult<TokenResponse>();

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

                return await Result.SuccessAsync();
            }

            return result;
        }

        public async Task<IResult> Logout()
        {
            //Usamos entre paréntesis porque el método MarkUserAsLoggedOut es propio de IGIft...provider

            await _localStorage.RemoveItemAsync(AppConstants.Local.AuthToken);
            await _localStorage.RemoveItemAsync(AppConstants.Local.RefreshToken);
            await _localStorage.RemoveItemAsync(AppConstants.Local.UserImageURL);
            await _localStorage.RemoveItemAsync(AppConstants.Local.IdUser);

            _httpClient.DefaultRequestHeaders.Authorization = null;
            await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return await Result.SuccessAsync();
        }

        /// <summary>
        /// Devuelve el token renovado o arroja una excepcion si no se pudo hacer la renovacion en el servidor
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(AppConstants.Local.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(AppConstants.Local.RefreshToken);


            var response = await _httpClient.PostAsJsonAsync(ConstTokenController.RefreshToken, new TokenRequest { Token = token!, RefreshToken = refreshToken! });
            var result = await response.ToResult<TokenResponse>();

            if (!result.Succeeded)
            {
                throw new Exception("error al refrescar el token");
            }

            token = result.Data.Token;
            refreshToken = result.Data.RefreshToken;

            await _localStorage.SetItemAsync(AppConstants.Local.AuthToken, token);
            //await _localStorage.SetItemAsync(AppConstants.Local.RefreshToken, refreshToken);

            await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();

            return token;
        }


        public async Task<string> TryRefreshToken()
        {
            var refreshTokenDisponible = await _localStorage.GetItemAsync<string>(AppConstants.Local.RefreshToken);

            if (string.IsNullOrEmpty(refreshTokenDisponible))
                return string.Empty;

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;

            if (diff.TotalMinutes <= 1)
                return await RefreshToken();

            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken() => await RefreshToken();

    }
}
