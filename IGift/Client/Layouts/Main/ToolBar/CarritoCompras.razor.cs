using IGift.Application.Responses.Pedidos;
using IGift.Client.Extensions;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class CarritoCompras
    {
        [Inject] IShopCart _carritoCompras { get; set; }

        [CascadingParameter] public HubConnection _hubConnection { get; set; }

        private List<PeticionesResponse> list { get; set; } = new();

        private int _peticiones { get; set; }
        public bool _open { get; set; }
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var result = await _carritoCompras.GetShopCartAsync();
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

            _hubConnection.On<ICollection<PeticionesResponse>>(AppConstants.SignalR.ReceiveShopCartNotificationAsync, async (lista) =>
            {
                //TODO es necesario enviar a esta subscripcion el Id si ya lo estamos localizando desde la clase signalR
                var idUser = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.IdUser);

                // Ejecutar el cambio de estado en el contexto de la IU

                list = lista.ToList();
                _peticiones = list.Count;
                _snack.Add("Peticion agregada al carrito", Severity.Success);
                await InvokeAsync(StateHasChanged);//Esto tiene que estar siempre porque dentro de un 'on' no se detecta un cambio en el UI
            });
        }
    }
}
