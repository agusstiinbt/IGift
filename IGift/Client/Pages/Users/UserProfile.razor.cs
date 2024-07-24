using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace IGift.Client.Pages.Users
{
    public partial class UserProfile
    {
        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }

        private string Correo { get; set; }

        protected override async Task OnInitializedAsync()
        {

            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                Nombre = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;

                Apellido = user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value!;

                Iniciales = Nombre.Substring(0, 1).ToUpper() + Apellido.Substring(0, 1).ToUpper();

                Correo = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value!;
            }
        }
    }
}
