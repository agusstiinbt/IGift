using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Client.Infrastructure.Services.Interceptor;

namespace IGift.Client.Shared
{
    public partial class MainLayout
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private ISnackbar _snack { get; set; }
        [Inject] private IHttpInterceptorManager _interceptor { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthService.Disconnect(DotNetObjectReference.Create(this));
            _interceptor.RegisterEvent();
        }

        [JSInvokable]
        public async Task Logout()
        {
            var authState = await AuthenticationState;
            if (authState.User.Identity!.IsAuthenticated)
            {
                _nav.NavigateTo(AppConstants.Routes.Logout);
            }
        }

        [JSInvokable]
        public async Task ShowMessage()
        {
            var authState = await AuthenticationState;
            if (authState.User.Identity!.IsAuthenticated)
            {
                _snack.Add("Su sesión está por expirar por inactividad", Severity.Warning);
            }
        }
    }
}
