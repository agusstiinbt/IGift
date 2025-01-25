using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Identity.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages.Users.Login
{
    public partial class Login
    {
        [Parameter] public string RegistrationSuccess { get; set; }

        private UserLoginRequest loginModel = new();

        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                _nav.NavigateTo("/");
            }

            if (RegistrationSuccess == "true")
            {
                _snack.Add("Registración de usuario exitosa", Severity.Success);
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
