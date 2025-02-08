using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using IGift.Client.Infrastructure.Services.Identity.Authentication;
using System.Runtime.InteropServices;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;


namespace IGift.Client.Extensions
{
    public static class HubExtensions
    {
        public static async Task<HubConnection> TryInitialize(
            this HubConnection hubConnection,
            NavigationManager navigationManager,
            IAuthService authService,
            ILocalStorageService localStorage)
        {

            var token = await localStorage.GetItemAsync<string>(AppConstants.Local.AuthToken);

            //var refreshToken = await IsTokenExpired(token!);
            //if (refreshToken)
            //    throw new Exception("401 Token expirado");

            if (hubConnection == null)
            {
                try
                {
                    hubConnection = new HubConnectionBuilder()
                        .WithUrl(navigationManager.ToAbsoluteUri(AppConstants.SignalR.HubUrl), options =>
                        {
                            options.AccessTokenProvider = async () => await Task.FromResult(token);
                        })
                        .WithAutomaticReconnect()
                        .Build();

                    // Manejo de eventos de reconexión y errores
                    hubConnection.Closed += async (error) =>
                    {
                        Console.WriteLine("SignalR desconectado. Reintentando conexión...");
                        await ReconnectWithRetryAsync(hubConnection);
                    };

                    // 🔥 IMPORTANTE: Intentar iniciar la conexión
                    if (hubConnection.State == HubConnectionState.Disconnected)
                    {
                        await hubConnection.StartAsync();
                        Console.WriteLine("Conexión SignalR establecida.");
                    }
                    return hubConnection;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return hubConnection;
        }

        /// <summary>
        /// Devuelve un string vacio si no hace falta renovar el token o si no existe un refreshToken en LocalStorage. Si falla la renovacion del token en el servidor arroja una Exception del metodo RefreshToken. Sino, devuelve el token renovado.
        /// </summary>
        /// <param name="authService"></param>
        /// <returns></returns>
        private static async Task<string> GetValidTokenAsync(IAuthService authService)
        {
            try
            {
                return await authService.TryRefreshToken();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        public static async Task ReconnectWithRetryAsync(HubConnection hubConnection)
        {
            while (hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await hubConnection.StartAsync();
                    Console.WriteLine("Reconectado exitosamente a SignalR.");
                    return;
                }
                catch
                {
                    Console.WriteLine("Falló la reconexión. Reintentando en 5 segundos...");
                    await Task.Delay(5000);
                }
            }
        }

        private static async Task<bool> IsTokenExpired(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwt = jwtHandler.ReadToken(token) as JwtSecurityToken;

            return jwt?.ValidTo < DateTime.UtcNow.AddMinutes(1); // Expira en menos de 1 minuto
        }
    }

}
