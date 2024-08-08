using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Client.Infrastructure.Services.Interceptor;
using Microsoft.AspNetCore.SignalR.Client;
using IGift.Client.Extensions;
using System.Security.Claims;

namespace IGift.Client.Layouts.Main
{
    public partial class MainLayout
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        private HubConnection _hubConnection;

        [Inject] private IAuthService _authService { get; set; }
        [Inject] private IHttpInterceptorManager _interceptor { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationState;
            if (authState.User.Identity!.IsAuthenticated)
            {
                await _authService.Disconnect(DotNetObjectReference.Create(this));
                ActivateHttpInterceptor();
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

        private void ActivateHttpInterceptor()
        {
            _interceptor.RegisterEvent();
        }


        /// <summary>
        /// Inicializamos todas las conexiones de tipo Hub
        /// </summary>
        /// <returns></returns>
        private async Task InitializeHub()
        {
            var currentUserId = AuthenticationState.Result.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            var nombre = AuthenticationState.Result.User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;

            _hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);

            await _hubConnection.StartAsync();

            _hubConnection.On<string, string, string>(AppConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
            {
                if (currentUserId == receiverUserId)
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
                        //TODO en el codigo de blazor hero tenian el código siguiente, fijarse si no es necesario descomentarlo, creo que no:
                        //   _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                await _hubConnection.SendAsync(AppConstants.SignalR.PingResponse, currentUserId, userName);

            });//TODO estudiar qué hace este

            await _hubConnection.SendAsync(AppConstants.SignalR.OnConnect, currentUserId);

            _snack.Add(string.Format("Bienvenido ", nombre), Severity.Success);

        }
    }
}
