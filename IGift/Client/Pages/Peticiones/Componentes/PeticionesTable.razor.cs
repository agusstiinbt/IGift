using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Pages.Peticiones.Componentes
{
    public partial class PeticionesTable
    {
        [CascadingParameter] private HubConnection _hubConnection { get; set; }

        private ICollection<PeticionesResponse> _pagedData;
        private MudTable<PeticionesResponse> _table;

        private int _totalItems;
        private int _currentPage;

        private string _searchString { get; set; } = string.Empty;
        private string Compra { get; set; } = "Compra";
        private string Venta { get; set; } = "Venta";

        private string EstiloBotonComprarPeticion { get; set; } = "background-color:#2A3038;color:white;";
        private string EstiloBotonCrear { get; set; } = "color:white;";
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
                EstiloBotonComprarPeticion = "background-color:#2A3038;color:white;";
                EstiloBotonCrear = "color:white;";
            }
            else
            {
                EstiloBotonCrear = "background-color:#2A3038;color:white;";
                EstiloBotonComprarPeticion = "color:white;";
            }

        }

        private async Task AgregarAlCarrito(PeticionesResponse p)
        {
            //TODO esto debería de ser un parámetro desde arriba para no invocarlo todo el tiempo
            var idUser = await _localStorage.GetItemAsync<string>(AppConstants.Local.IdUser);
            var response = await _shopCartService.SaveShopCartAsync(p);
            if (response.Succeeded)
            {
                await _hubConnection.SendAsync(AppConstants.SignalR.SendShopCartNotificationAsync, _pagedData, idUser);
            }
            else
            {
                _snack.Add(response.Messages.FirstOrDefault());
            }
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

            var request = new GetAllPeticionesQuery { PageNumber = pageNumber, PageSize = pageSize, Descripcion = _searchString };
            var response = await _peticionesService.GetAll(request);
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
