using System.Security.Claims;
using Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class TogglePerfil
    {
        [Parameter] public string userName { get; set; }

        private string IdUser { get; set; } = string.Empty;
        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }

        private string imageBase64 = string.Empty;

        private string background = string.Empty;
        private string Correo { get; set; }
        
        public bool _open;
        public bool _expanded;

        private void Logout()
        {
            _nav.NavigateTo(IGift.Shared.Constants.AppConstants.Routes.Logout);

        }

        public void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }


        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                Nombre = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
                Apellido = user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value!;
                Correo = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value!;
                IdUser = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

            }
            await GetProfilePicture();
        }
        private async Task GetProfilePicture()
        {
            var result = await _profileService.GetByIdAsync(IdUser!);

            if (result.Succeeded)
            {
                imageBase64 = Convert.ToBase64String(result.Data.Data);
                background = $"background-image: url('data:image/jpg;base64,{imageBase64}');background-size:cover";
            }
            else
            {
                Iniciales = Nombre.Substring(0, 1).ToUpper() + Apellido.Substring(0, 1).ToUpper();
            }
        }
    }
}
