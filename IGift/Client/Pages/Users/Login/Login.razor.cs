using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Identity.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages.Users.Login
{
    public partial class Login
    {

        private UserLoginRequest loginModel = new();

        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (user.Identity!.IsAuthenticated)
            {
                _nav.NavigateTo("/");
            }
        }

        private async Task HandleLoginForm()
        {
            var result = await _authService.Login(loginModel);

            if (result.Succeeded)
            {
                _nav.NavigateTo(IGift.Shared.Constants.AppConstants.Routes.Home);
            }
            else
            {
                _snack.Add(result.Messages.FirstOrDefault(), Severity.Error);
            }
        }
    }
}
