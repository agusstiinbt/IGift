using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace IGift.Client.Pages.Users
{
    public partial class UserProfile
    {
        private string NombreUsuario { get; set; }

        protected override async Task OnInitializedAsync()
        {

            var state = await((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                NombreUsuario = state.User.Identity.Name;
            }
        }
    }
}
