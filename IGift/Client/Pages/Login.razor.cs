using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Login
    {
        [Inject] private IAuthService? AuthService { get; set; }
        [Inject] private ISnackbar? _snackBar { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }

        private UserLoginRequest loginModel = new UserLoginRequest();

        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
           

            if (state != null && user.Identity.IsAuthenticated)
            {
                _navigationManager.NavigateTo("/");
            }
        }
        private async Task HandleLogin()
        {
            var result = await AuthService.Login(loginModel);

            if (result.Succeeded)
            {
                NavigationManager.NavigateTo(AppConstants.Routes.Home);
            }
            else
            {
                _snackBar.Add(result.Messages.FirstOrDefault(), Severity.Error);
            }
        }
    }
}
