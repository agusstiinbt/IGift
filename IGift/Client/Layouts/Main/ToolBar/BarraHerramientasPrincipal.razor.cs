using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class BarraHerramientasPrincipal
    {
        private string NombreUsuario { get; set; }

        public string EstiloBotones { get; set; } = "color:#FFCC09";


        [CascadingParameter] private HubConnection _hubConnection { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                NombreUsuario = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
            }

            _hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }
    }
}