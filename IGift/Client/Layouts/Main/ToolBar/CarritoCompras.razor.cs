using IGift.Application.Responses.Peticiones;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class CarritoCompras
    {
        [CascadingParameter] private HubConnection? _hubConnection { get; set; }

        private List<PeticionesResponse> list { get; set; } = new();

        private int _peticiones { get; set; } = 0;
        public bool _open { get; set; }
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var result = await _shopCartService.GetShopCartAsync();
            if (result.Succeeded)
            {
                list = result.Data;
                _peticiones = list.Count;
            }
            _visible = _peticiones == 0 ? false : true;

            await InitializeHub();

            _visible = _peticiones == 0 ? false : true;
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
            if (_hubConnection != null)
            {
                _hubConnection.On<ICollection<PeticionesResponse>>(AppConstants.SignalR.ReceiveShopCartNotificationAsync, async (lista) =>
                {

                    // Ejecutar el cambio de estado en el contexto de la UI
                    list = lista.ToList();
                    _peticiones = list.Count;
                    _snack.Add("Peticion agregada al carrito", Severity.Success);

                    await InvokeAsync(StateHasChanged); // Asegurar que la actualización de UI ocurre en el hilo correcto
                });
            }

            await Task.CompletedTask; // Para evitar advertencias de métodos async sin 'await'
        }

    }
}
