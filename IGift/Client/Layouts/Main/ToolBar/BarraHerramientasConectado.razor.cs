using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class BarraHerramientasConectado
    {
        [Parameter]
        public string userName { get; set; }

        [CascadingParameter]
        private string _estiloBotones { get; set; }

        [Parameter] public HubConnection _hubConnection { get; set; }

        private string href { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(userName))
            {
                var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                userName = state.User.GetFirstName();
            }
            _hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public bool _open;

        public void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }

        private string MenuStyle = "visibility:collapse";
        private void ShowMenu()
        {
            if (!string.IsNullOrEmpty(MenuStyle))
            {
                MenuStyle = string.Empty;
            }
            else
            {
                MenuStyle = "visibility:collapse";
            }
        }

        bool _expanded;
    }
}
