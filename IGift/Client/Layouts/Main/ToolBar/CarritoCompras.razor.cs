using IGift.Application.Responses.Peticiones;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class CarritoCompras
    {
        [CascadingParameter] private HubConnection? _hubConnection { get; set; }

        private List<PeticionesResponse> list { get; set; } = new List<PeticionesResponse>();
        private int _peticiones { get; set; } = 0;
        public bool _open { get; set; }
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            list = await _shopCartService.GetShopCartAsync();

            _peticiones = list.Count;

            _visible = _peticiones > 0;

            await InitializeHub();
        }
        private void ToggleOpen() => _open = !_open;

        private async Task InitializeHub()
        {
            await Task.FromResult(_hubConnection!.On<PeticionesResponse>(AppConstants.SignalR.ReceiveShopCartNotificationAsync, (p) =>
            {
                list.Add(p);
                _peticiones++;
                _visible = _peticiones > 0;
                InvokeAsync(StateHasChanged);
            }));
        }
    }
}
