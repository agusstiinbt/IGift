using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using IGift.Application.Requests.Identity;
using Client.Infrastructure.Extensions;
using IGift.Shared;
using Microsoft.JSInterop;
using static IGift.Shared.AppConstants.Endpoints;
using MudBlazor;

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
            //using var content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsJsonAsync(Endpoints.Users.Register, registerModel);
            //var result = await response.Content.ReadFromJsonAsync<IResult>();
            //return result!;
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Endpoints.Users.Register, registerModel);
            return await response.ToResult();
        }

        /// <summary>
        /// Genera un inicio de sesión en el servidor y si es exitoso guarda las credenciales del usuario en el cliente
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>El resultado de la operación</returns>
        public async Task<IResult> Login(UserLoginRequest loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(Token.GetToken, loginModel);
            var result = await response.ToResult<TokenResponse>();

            if (result.Succeeded)
            {
                //separamos las propiedades
                var token = result!.Data.Token;
                var refreshToken = result!.Data.RefreshToken;
                var userPicture = result!.Data.UserImageURL;
                var time = result!.Data.RefreshTokenExpiryTime;

                //gurdamos cada propiedad en el cliente
                await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.AuthToken, token);
                await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.RefreshToken, refreshToken);
                await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.UserImageURL, userPicture);
                await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.ExpiryTime, time.ToString());


                //preparamos los headers con el token correcto
                await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();

                // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                return await Result.SuccessAsync();
            }

            return await Result.FailAsync("Error al iniciar sesión");
        }

        /// <summary>
        /// Removemos del cliente las credenciales obtenidas a través del token y se cierra la sesión del usuario
        /// </summary>
        /// <returns>Resultado de la operacion</returns>
        public async Task<IResult> Logout()
        {
            await _localStorage.RemoveItemAsync(AppConstants.StorageConstants.Local.AuthToken);
            await _localStorage.RemoveItemAsync(AppConstants.StorageConstants.Local.RefreshToken);
            await _localStorage.RemoveItemAsync(AppConstants.StorageConstants.Local.UserImageURL);

            //Usamos entre paréntesis porque el método MarkUserAsLoggedOut es propio de IGIft...provider
            ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            return await Result.SuccessAsync();
        }

        public async Task Disconnect<T>(DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            await _js.InitializeInactivityTimer(dotNetObjectReference);
        }

        //public async Task<string> RefreshToken()
        //{
        //    var token = await _js.ObtenerDeLocalStorage<string>(AppConstants.StorageConstants.Local.AuthToken);
        //    var refreshToken = await _js.ObtenerDeLocalStorage<string>(AppConstants.StorageConstants.Local.RefreshToken);

        //    return null;
        //}

        public async Task<string> TryRefreshToken()
        {
            var tokenDisponible = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.RefreshToken);
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
            var token = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.RefreshToken);

            //TODO fijarse si usar postasync o postasjsonasync
            var response = await _httpClient.PostAsJsonAsync(Token.RefreshToken, new TokenRequest { Token = token!, RefreshToken = refreshToken! });
            var result = await response.ToResult<TokenResponse>();

            if (!result.Succeeded)
            {
                throw new Exception("error al refrescar el token");
            }

            token = result.Data.Token;
            refreshToken = result.Data.RefreshToken;

            await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(AppConstants.StorageConstants.Local.RefreshToken, refreshToken);
            //TODO fijarse si este código de abajo puede ser reemplazado con el StateChangedAsync
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return token;
        }
    }
}
