using System.Net.Http.Headers;
using System.Security.Claims;
using Client.Infrastructure.Authentication;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Client.Extensions;
using IGift.Client.Pages.Communication.Chat;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace IGift.Client.Pages.Inicio
{
    public partial class Index : IAsyncDisposable
    {
        // Parametros
        [CascadingParameter] public required Task<AuthenticationState> AuthenticationState { get; set; }
        [Parameter] public string? _Categoria { get; set; }
        [Parameter] public string? TxtBusqueda { get; set; } = string.Empty;
        [Parameter] public PaginatedResult<PeticionesResponse>? _datosDeBusqueda { get; set; } = null;

        //Propiedades

        public HubConnection? _hubConnection { get; set; }
        private DotNetObjectReference<Index>? _dotNetRef { get; set; }

        private MudTable<PeticionesResponse> _table;
        private PaginatedResult<PeticionesResponse>? peticiones { get; set; } = null;

        private ICollection<PeticionesResponse> _pagedData { get; set; }
        private List<CategoriaResponse> listaCategorias = new List<CategoriaResponse>();
        private List<TitulosConectadoResponse> titulosConectado = new List<TitulosConectadoResponse>();

        //Strings
        public string SearchString { get; set; } = string.Empty;
        private string CurrentUserId { get; set; } = string.Empty;
        private string Compra { get; set; } = "Compra";
        private string Venta { get; set; } = "Venta";
        private string EstiloBotonComprarPeticion { get; set; } = "background-color:#2A3038;color:white;";
        private string EstiloBotonCrear { get; set; } = "color:white;";
        private string EstiloCrypto { get; set; } = "background-color:#181A20;color:white;";
        private string BotonSeleccionado { get; set; } = "USDT";
        public string UserName { get; set; }
        private string href { get; set; }

        //Booleans
        public bool ShowTablePeticiones { get; set; } = false;
        public bool IsHubConnected { get; set; } = false;

        //Ints
        private int _totalItems;
        private int _currentPage;
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationState;
            if (authState.User.Identity!.IsAuthenticated)
            {
                //_interceptor.RegisterEvent(); //TODO quitar? Leer la descripcion de ese metodo
                await InitializeHub();

                if (IsHubConnected)
                {
                    _dotNetRef = DotNetObjectReference.Create(this);

                    await _JS.InitializeInactivityTimer(_dotNetRef);

                    var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();

                    var response = await _titulosService.LoadConectado();
                    if (response.Succeeded)
                    {
                        titulosConectado = response.Data.Titulos.ToList();
                        listaCategorias = response.Data.Categorias.ToList();

                        UserName = state.User.GetFirstName();
                    }

                }
                else
                {
                    try
                    {
                        var tokenRenovado = await _authService.TryForceRefreshToken();
                        _snack.Add("Token renovado");
                        await Task.Delay(1500);
                        _nav.NavigateTo(AppConstants.Routes.Home);
                    }
                    catch (Exception e)
                    {
                        await Logout();
                    }
                }
            }
            else
            {
                _nav.ToAbsoluteUri(AppConstants.Routes.Logout);
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

        /// <summary>
        /// Si estamos autenticados agregamos el item al carrito, sino debemos hacer un Login
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private async Task AgregarAlCarrito(PeticionesResponse p)
        {
            if (_hubConnection != null)
            {
                var idUser = await _localStorage.GetItemAsync<string>(AppConstants.Local.IdUser);
                var response = await _shopCartService.SaveShopCartAsync(p);

                if (response.Succeeded)
                {
                    await _hubConnection.SendAsync(AppConstants.SignalR.SendShopCartNotificationAsync, p, idUser);

                    // Notificación en el UI thread
                    await InvokeAsync(() => _snack.Add("Petición agregada al carrito", Severity.Success));
                }
                else
                    _snack.Add(response.Messages.FirstOrDefault());
            }
            else
                _nav.NavigateTo(AppConstants.Routes.Login);
        }

        public async Task RealizarBusqueda()
        {
            TxtBusqueda = SearchString;

            if (!ShowTablePeticiones)
                ShowTablePeticiones = true;
            else
                await OnSearch(TxtBusqueda);
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


            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });
            _pagedData.Add(new PeticionesResponse() { Descripcion = "Tarjeta de regalo Automotriz", Monto = 123, Moneda = "USDT" });


            return new TableData<PeticionesResponse> { TotalItems = _totalItems, Items = _pagedData }; ;
        }
        private async Task OnSearch(string text)
        {
            TxtBusqueda = text;
            await _table.ReloadServerData();
        }

        /// <summary>
        /// Inicializamos todas las conexiones de tipo Hub
        /// </summary>
        /// <returns></returns>
        private async Task InitializeHub()
        {
            try
            {
                _hubConnection = await _hubConnection.TryInitialize(_nav, _authService, _localStorage);
                if (_hubConnection != null)
                {
                    _hubConnection.On(AppConstants.SignalR.ReceiveRegenerateTokensAsync, async () =>
                    {
                        try
                        {
                            var token = await _authService.TryForceRefreshToken();
                            if (!string.IsNullOrEmpty(token))
                            {
                                _snack.Add("Token refrescado", Severity.Success);
                                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            }
                        }
                        catch (Exception)
                        {
                            _snack.Add("Sesión finalizada", Severity.Error);
                            await _authService.Logout();
                            _nav.NavigateTo("/");
                        }
                    });

                    _hubConnection.On<ChatHistoryResponse>(AppConstants.SignalR.ReceiveChatNotificationAsync, (chatHistory) =>
                    {
                        if (CurrentUserId == chatHistory.ToUserId)
                        {
                            _JS.InvokeAsync<string>("PlayAudio", "notification");
                            _snack.Add("Has recibido un mensaje de " + chatHistory.UserName, Severity.Info, config =>
                            {
                                config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
                                config.VisibleStateDuration = 10000;
                                config.Onclick = snackbar =>
                                {
                                    _nav.NavigateTo($"chat/{chatHistory.FromUserId}");
                                    return Task.CompletedTask;
                                };
                            });


                            //await _hubConnection.SendAsync(AppConstants.SignalR.SendChatNotificationAsync, "Nuevo mensaje de " + userName, ToUserId, CurrentUserId);
                        }
                        StateHasChanged();
                    });


                    //TODO investigar sobre esto porque puede ser util para desloguer automaticamente cuando se le haya cambiado permisos o roles a un usuario
                    //_hubConnection.On<string, string>(AppConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
                    //{
                    //    if (currentUserId != userId)
                    //    {
                    //        var rolesResponse = await RoleManager.GetRolesAsync();
                    //        if (rolesResponse.Succeeded)
                    //        {
                    //            var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                    //            if (role != null)
                    //            {
                    //                var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
                    //                if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                    //                {
                    //                    _snackBar.Add(_localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
                    //                    await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
                    //                    await _authenticationManager.Logout();
                    //                    _navigationManager.NavigateTo("/login");
                    //                }
                    //            }
                    //        }
                    //    }
                    //});

                    _hubConnection.On<string>(AppConstants.SignalR.PingRequest, async (userName) =>
                    {
                        await _hubConnection.SendAsync(AppConstants.SignalR.PingResponse, CurrentUserId, userName);
                    });

                    CurrentUserId = AuthenticationState.Result.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

                    await _hubConnection.SendAsync(AppConstants.SignalR.OnConnect, CurrentUserId);

                    IsHubConnected = true;
                }
            }
            catch (Exception e)
            {
                IsHubConnected = false;
                if (e.Message.Contains("401"))
                    _snack.Add("Conexion con SignalR perdida. Proceda con cuidado", Severity.Info, config =>
                    {
                        config.VisibleStateDuration = 3000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.ActionColor = Color.Primary;
                    });
                else
                    _snack.Add(e.Message);
            }
        }

        /// <summary>
        /// Este método es invocado por javascript en el caso de que se haya excedido el tiempo de inactividad y nos deslogueamos
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public async Task Logout()
        {
            _snack.Add("Sesion terminada. Vuelva a iniciar sesion por favor");
            await Task.Delay(3000);
            _nav.NavigateTo(AppConstants.Routes.Logout);
        }

        /// <summary>
        /// Este mensaje es invocado por javascript en el caso de que tengamos tiempo de inactividad
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public async Task ShowMessage()
        {
            var authState = await AuthenticationState;
            if (authState.User.Identity!.IsAuthenticated)
            {
                _snack.Add("Su sesión está por expirar por inactividad", Severity.Warning);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _dotNetRef?.Dispose();
            _interceptor.DisposeEvent();
            await _hubConnection!.DisposeAsync();
        }
    }
}
