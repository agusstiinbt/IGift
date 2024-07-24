using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Login
    {
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }

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

    }
}
