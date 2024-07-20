using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Client.Infrastructure.Authentication;

namespace IGift.Client.Shared.BarraHerramientas
{
    public partial class BarraHerramientasPrincipal
    {
        [Inject] private IUserManager? _userManager { get; set; }
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }

        private string NombreUsuario { get; set; }

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
