using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Infrastructure.Authentication
{
    public class IGiftAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        public IGiftAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorage = localStorageService;
        }
        public ClaimsPrincipal AuthenticationStateUser { get; set; }

        /// <summary>
        /// Este método actualiza el estado de autenticación del usuario en toda la aplicación. Cuando hagamos un Login o un refresh token se debe invocar este método que notifica cuando se haya actualizado el estado de autenticación y prepara los headers del cliente HTTP
        /// </summary>
        /// <returns></returns>
        public async Task StateChangedAsync()
        {
            var authState = Task.FromResult(await GetAuthenticationStateAsync());

            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Desloguea al usuario dejando el estado de autenticación del usuario vacío y limpia los headers del HTTP
        /// </summary>
        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.ClearAsync();//en teoria esto liompia todo lo que se encuentra en el local storage

            //await _localStorage.RemoveItemAsync(AppConstants.Local.AuthToken);
            //await _localStorage.RemoveItemAsync(AppConstants.Local.RefreshToken);
            //await _localStorage.RemoveItemAsync(AppConstants.Local.UserImageURL);
            //await _localStorage.RemoveItemAsync(AppConstants.Local.IdUser);

            _httpClient.DefaultRequestHeaders.Authorization = null;

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Devuelve el estado de autenticación de usuario y prepara los headers del client HTTP con el token
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(AppConstants.Local.AuthToken);

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromToken(savedToken), "jwt")));
            AuthenticationStateUser = state.User;
            return state;
        }

        /// <summary>
        /// Genera una colección de Claims DE un token
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        private IEnumerable<Claim> GetClaimsFromToken(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                        claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                //TODO implementacion de permisos para más adelante
                //keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
                //if (permissions != null)
                //{
                //    if (permissions.ToString().Trim().StartsWith("["))
                //    {
                //        var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                //        claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
                //    }
                //    else
                //    {
                //        claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()));
                //    }
                //    keyValuePairs.Remove(ApplicationClaimTypes.Permission);
                //}

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string payload)
        {
            payload = payload.Trim().Replace('-', '+').Replace('_', '/');
            var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            return Convert.FromBase64String(base64);
        }
    }
}

