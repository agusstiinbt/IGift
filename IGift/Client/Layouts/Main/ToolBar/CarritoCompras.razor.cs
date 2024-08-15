using Client.Infrastructure.Services.Notification;
using IGift.Application.Requests.LocalesAdheridos.Command;
using IGift.Application.Requests.Peticiones.Command;
using IGift.Application.Responses.Notification;
using IGift.Client.Extensions;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class CarritoCompras
    {

        [Inject] ICarritoComprasService _carritoCompras { get; set; }

        private List<AddEditPeticionesCommand> list { get; set; } = new();

        [CascadingParameter] private HubConnection _hubConnection { get; set; }

        private int _peticiones { get; set; } = 0;
        public bool _open;
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var result = await _carritoCompras.ObtenerCarritoDePeticiones();
            if (result.Succeeded)
            {
                list = result.Data;
                _peticiones = list.Count;
            }
            _visible = _peticiones == 0 ? false : true;

            await InitializeHub();
        }

        private void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }

        private async Task InitializeHub()
        {
            _hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }

            _hubConnection.On<List<AddEditPeticionesCommand>>(AppConstants.SignalR.ReceiveCarritoComprasNotification, (lista) =>
            {
                _peticiones = lista.Count;
                list = lista;
                _snack.Add("Peticion agregada al carrito", Severity.Success);
                //StateHasChanged(); TODO agregar esto?
            });
        }
    }
}
