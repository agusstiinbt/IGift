using System.Security.Claims;
using Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class BarraHerramientasPrincipal
    {
        private string NombreUsuario { get; set; }

        public readonly string EstiloBotones = "color:black";

        [CascadingParameter] private HubConnection _hubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity!.IsAuthenticated)
            {
                NombreUsuario = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
            }
            try
            {
                //_hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);

                //if (_hubConnection.State == HubConnectionState.Disconnected)
                //{
                //    await _hubConnection.StartAsync();
                //}

            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}