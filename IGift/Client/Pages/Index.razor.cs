using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Index
    {
        [Inject] private IUserManager? _userManager { get; set; }
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }

        private string NombreUsuario { get; set; }

        public async void HandleSubmit()
        {
            var result = await _userManager.GetUsersAsync();
            if (!result.Succeeded)
            {
                _snack.Add(result.Messages.ToString(), Severity.Error);
            }
            _snack!.Add("Consulta exitosa");
        }
        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                NombreUsuario = state.User.Identity.Name;
            }
        }
    }
}
