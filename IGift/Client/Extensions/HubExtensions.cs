using Blazored.LocalStorage;
using IGift.Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

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

    }
}
