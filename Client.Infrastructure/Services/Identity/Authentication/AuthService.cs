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

        public async Task<IResult> Register(UserCreateRequest registerModel)
        {
            //using var content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsJsonAsync(Endpoints.Users.Register, registerModel);
            //var result = await response.Content.ReadFromJsonAsync<IResult>();
            //return result!;
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Users.Register, registerModel);
            return await response.ToResult();
        }

        /// <summary>
        /// Genera un inicio de sesión en el servidor y si es exitoso guarda las credenciales del usuario en el cliente
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>El resultado de la operación</returns>
        public async Task<IResult> Login(UserLoginRequest loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync(AppConstants.Users.LogIn, loginModel);
            var result = await response.ToResult<ApplicationUserResponse>();

            if (result.Succeeded)
            {
                //separamos las propiedades
                var token = result!.Data.Token;
                var refreshToken = result!.Data.RefreshToken;
                var userPicture = result!.Data.UserImageURL;
                var time = result!.Data.RefreshTokenExpiryTime;

                //gurdamos cada propiedad en el cliente
                await _localStorage.SetItemAsync("authToken", token);
                await _localStorage.SetItemAsync("refreshToken", refreshToken);
                await _localStorage.SetItemAsync("userImage", userPicture);
                await _localStorage.SetItemAsync("expiryTime", time);

                //preparamos los headers con el token correcto
                ((IGiftAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggin(token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
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
            await _js.InitializeInactivityTimer(dotNetObjectReference);
        }
    }
}
