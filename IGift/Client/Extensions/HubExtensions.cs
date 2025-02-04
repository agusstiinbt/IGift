using Blazored.LocalStorage;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Extensions
{
    public static class HubExtensions
    {
        public static async Task<HubConnection> TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager, ILocalStorageService _localStorage)
        {
            if (hubConnection == null)
            {
                await Task.FromResult(
                              hubConnection = new HubConnectionBuilder()
                                                .WithUrl(navigationManager.ToAbsoluteUri(AppConstants.SignalR.HubUrl), options =>
                                                {
                                                    options.AccessTokenProvider = async () => (await _localStorage.GetItemAsync<string>(AppConstants.Local.AuthToken));
                                                })
                                                .WithAutomaticReconnect()
                                                .Build());
            }
            return hubConnection;
        }
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                  .WithUrl(navigationManager.ToAbsoluteUri(AppConstants.SignalR.HubUrl))
                                  .Build();
            }
            return hubConnection;
        }
    }
}
