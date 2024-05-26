using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IGift.Client.Shared
{
    public partial class MainLayout
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }
        
        [Inject] private IAuthService AuthService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var respones = await AuthService.Disconnect(DotNetObjectReference.Create(this));
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
    }
}
