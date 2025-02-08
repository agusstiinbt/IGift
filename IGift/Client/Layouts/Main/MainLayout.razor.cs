using System.Net.Http.Headers;
using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace IGift.Client.Layouts.Main
{
    public partial class MainLayout
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        public HubConnection _hubConnection { get; set; }

        private bool _isConnected = false;
        protected override async Task OnInitializedAsync()
        {
            await _authService.Disconnect(DotNetObjectReference.Create(this));
            _interceptor.RegisterEvent();

            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
            if (state.User.Identity!.IsAuthenticated)
            {
                var curentUserId = AuthenticationState.Result.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
                var nombre = AuthenticationState.Result.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
                await InitializeHub();
            }
        }

        /// <summary>
        /// Este método es invocado por javascript en el caso de que se haya excedido el tiempo de inactividad y nos deslogueamos
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public async Task Logout()
        {
            var authState = await AuthenticationState;
            if (authState.User.Identity!.IsAuthenticated)
            {
                _nav.NavigateTo(AppConstants.Routes.Logout);
            }
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

        /// <summary>
        /// Inicializamos todas las conexiones de tipo Hub
        /// </summary>
        /// <returns></returns>
        private async Task InitializeHub()
        {

            try
            {
                _hubConnection = await _hubConnection.TryInitialize(_nav, _authService, _localStorage);

                _hubConnection.On<string, string, string>(AppConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
                {
                    if (curentUserId == receiverUserId)
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
                    await _hubConnection.SendAsync(AppConstants.SignalR.PingResponse, curentUserId, userName);
                });

                await _hubConnection.SendAsync(AppConstants.SignalR.OnConnect, curentUserId);

                _isConnected = true;

                _snack.Add("Bienvenido " + nombre, Severity.Success);//TODO cada vez que se refresca la página se muetra y solo debe ejecutarse una sola vez, fijarse cómo podemos solucionarlo
            }
            catch (Exception e)
            {
                if (e.Message.Contains("401"))
                {
                    _snack.Add("Conexion con SignalR perdida. Token vencido", Severity.Warning);
                }
                else
                {
                    _snack.Add("Ocurrio un eror inesperado de SignalR");
                }
            }
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
        }
    }
}
