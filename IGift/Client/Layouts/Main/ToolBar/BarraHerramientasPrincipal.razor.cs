using Client.Infrastructure.Authentication;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class BarraHerramientasPrincipal
    {
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