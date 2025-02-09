using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Identity.Users;
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
                StateHasChanged();
                await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).StateChangedAsync();
                _nav.NavigateTo(IGift.Shared.Constants.AppConstants.Routes.Home);
                StateHasChanged();
                await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).StateChangedAsync();
            }
            else
            {
                _snack.Add(result.Messages.FirstOrDefault(), Severity.Error);
            }
        }
    }
}
