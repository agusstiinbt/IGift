using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Pages.Inicio
{
    public partial class Index
    {
        [CascadingParameter] private HubConnection _hubConnection { get; set; }


        [Parameter]
        public string? _Categoria { get; set; }

        [Parameter]
        public string? TxtBusqueda { get; set; } = string.Empty;

        [Parameter]
        public PaginatedResult<PeticionesResponse>? _datosDeBusqueda { get; set; } = null;
        private PaginatedResult<PeticionesResponse>? peticiones { get; set; } = null;
        public ICollection<PeticionesResponse> _pagedData { get; set; }

        private MudTable<PeticionesResponse> _table;


        private string NombreUsuario { get; set; }

        public readonly string EstiloBotones = "color:black";
        private string Compra { get; set; } = "Compra";
        private string Venta { get; set; } = "Venta";

        private string EstiloBotonComprarPeticion { get; set; } = "background-color:#2A3038;color:white;";
        private string EstiloBotonCrear { get; set; } = "color:white;";
        private string EstiloCrypto { get; set; } = "background-color:#181A20;color:white;";
        private string BotonSeleccionado { get; set; } = "USDT";

        private int _totalItems;
        private int _currentPage;

        protected override async Task OnInitializedAsync()
        {
            _interceptor.RegisterEvent();

            try
            {
                _hubConnection = await _hubConnection.TryInitialize(_nav, _localStorage);

                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                }

                var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                var user = state.User;
                if (state != null && user.Identity!.IsAuthenticated)
                {
                    NombreUsuario = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
                }
            }
            catch (Exception e)
            {
                //TODO estudiar hubconnection y resolver los problemas
                throw e;
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
            var query = new GetAllPeticionesQuery()
            {
                SearchString = TxtBusqueda == null ? string.Empty : TxtBusqueda,
                Categoria = _Categoria == null ? string.Empty : _Categoria
            };

            var response = await _peticionesService.GetAll(query);
            peticiones = response;

            _totalItems = peticiones.TotalCount;
            _currentPage = peticiones.CurrentPage;
            _pagedData = peticiones.Data;

            return new TableData<PeticionesResponse> { TotalItems = _totalItems, Items = _pagedData }; ;
        }
        private async Task OnSearch(string text)
        {
            TxtBusqueda = text;
            await _table.ReloadServerData();
        }
    }
}
