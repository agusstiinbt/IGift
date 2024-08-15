using Client.Infrastructure.Services.Requests;
using IGift.Application.Requests.Peticiones.Query;
using IGift.Application.Responses.Pedidos;
using IGift.Client.Extensions;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Pages.Peticiones.Componentes
{
    public partial class PeticionesTable
    {
        //TODO limpiar código que no se esté utilizando
        [Inject] IPeticionesService _peticiones { get; set; }
        [Inject] ICarritoComprasService _carritoService { get; set; }

        [CascadingParameter] private HubConnection _hubConnection { get; set; }

        private IEnumerable<PeticionesResponse> _pagedData;
        private MudTable<PeticionesResponse> _table;

        private int _totalItems;
        private int _currentPage;

        private string _searchString { get; set; } = string.Empty;
        private string Compra { get; set; } = "Compra";
        private string Venta { get; set; } = "Venta";

        private string EstiloBotonCompra { get; set; } = "background-color:#2A3038;color:white;";
        private string EstiloBotonVenta { get; set; } = "color:white;";
        private string EstiloCrypto { get; set; } = "background-color:#181A20;color:white;";
        private string BotonSeleccionado { get; set; } = "USDT";

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        private void SeleccionarBoton(string boton)
        {
            BotonSeleccionado = boton;
        }

        private string GetEstiloBoton(string boton)
        {
            if (BotonSeleccionado == boton)
            {
                return "background-color:#181A20;color:yellow;";
            }
            return EstiloCrypto;
        }

        private void TipoOperacion(string boton)
        {
            if (boton == Compra)
            {
                EstiloBotonCompra = "background-color:#2A3038;color:white;";
                EstiloBotonVenta = "color:white;";
            }
            else
            {
                EstiloBotonVenta = "background-color:#2A3038;color:white;";
                EstiloBotonCompra = "color:white;";
            }

        }

        private async Task AgregarAlCarrito(PeticionesResponse p)
        {
            await _carritoService.GuardarEnCarritoDeCompras(p);
            await _hubConnection.SendAsync(AppConstants.SignalR.SendCarritoComprasNotificationAsync, _pagedData);

        }

        private async Task<TableData<PeticionesResponse>> GetData(TableState state, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<PeticionesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }


        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            //TODO implementar el ordenamiento
            // public GetAllPeticionesQuery(int pageNumber, int pageSize, string searchString, string[] orderBy)

            var request = new GetAllPeticionesQuery { PageNumber = pageNumber, PageSize = pageSize, SearchString = _searchString };
            var response = await _peticiones.GetAll(request);
            if (response.Succeeded)
            {
                _totalItems = response.Data.TotalCount;
                _currentPage = response.Data.CurrentPage;
                _pagedData = response.Data.Data;
            }
            else
            {
                foreach (var messages in response.Messages)
                {
                    _snack.Add(messages, Severity.Error);
                }
            }
        }
        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }//Esto no borrarlo porque se va a utilizar más adelante
    }
}
