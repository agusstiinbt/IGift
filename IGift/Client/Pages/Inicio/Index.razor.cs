using System.Net.Http.Headers;
using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Peticiones.Query;
using IGift.Application.Responses.Peticiones;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace IGift.Client.Pages.Inicio
{
    public partial class Index
    {
        // Parametros
        [CascadingParameter] public required Task<AuthenticationState> AuthenticationState { get; set; }
        [Parameter] public string? _Categoria { get; set; }
        [Parameter] public string? TxtBusqueda { get; set; } = string.Empty;
        [Parameter] public PaginatedResult<PeticionesResponse>? _datosDeBusqueda { get; set; } = null;

        //Propiedades
        private PaginatedResult<PeticionesResponse>? peticiones { get; set; } = null;
        public ICollection<PeticionesResponse> _pagedData { get; set; }
        private MudTable<PeticionesResponse> _table;
        public HubConnection? _hubConnection { get; set; }

        //Strings
        public string SearchString { get; set; } = string.Empty;
        private string NombreUsuario { get; set; } = string.Empty;
        private string CurrentUserId { get; set; } = string.Empty;
        private string Compra { get; set; } = "Compra";
        private string Venta { get; set; } = "Venta";
        public string EstiloBotones { get; set; } = string.Empty;
        private string EstiloBotonComprarPeticion { get; set; } = "background-color:#2A3038;color:white;";
        private string EstiloBotonCrear { get; set; } = "color:white;";
        private string EstiloCrypto { get; set; } = "background-color:#181A20;color:white;";
        private string BotonSeleccionado { get; set; } = "USDT";

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
                _interceptor.RegisterEvent();
                IsHubConnected = await InitializeHub();

                if (!IsHubConnected)
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
                else
                {
                    //await _authService.Disconnect(DotNetObjectReference.Create(this));
                    var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                    NombreUsuario = state.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
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
        private async Task<bool> InitializeHub()
        {
            try
            {
                _hubConnection = await _hubConnection.TryInitialize(_nav, _authService, _localStorage);
                if (_hubConnection != null)
                {
                    _hubConnection.On<string, string, string>(AppConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
                    {
                        if (CurrentUserId == receiverUserId)
                        {
                            //TODO implementar:..._jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                            _snack.Add(message, Severity.Info, config =>
                            {
                                config.VisibleStateDuration = 10000;
                                config.HideTransitionDuration = 500;
                                config.ShowTransitionDuration = 500;
                                config.Action = "Chat?";
                                config.ActionColor = Color.Primary;
                                config.Onclick = snackbar =>
                                {
                                    _nav.NavigateTo($"chat/{senderUserId}");
                                    return Task.CompletedTask;
                                };
                            });
                        }
                    });

                    _hubConnection.On(AppConstants.SignalR.ReceiveRegenerateTokens, async () =>
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
                        catch (Exception e)
                        {
                            _snack.Add("Sesión finalizada", Severity.Error);
                            await _authService.Logout();
                            _nav.NavigateTo("/");
                        }
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
            return IsHubConnected;
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

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }
    }
}
