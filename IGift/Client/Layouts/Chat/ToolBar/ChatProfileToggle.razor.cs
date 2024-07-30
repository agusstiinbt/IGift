using Client.Infrastructure.Authentication;
using System.Security.Claims;

namespace IGift.Client.Layouts.Chat.ToolBar
{
    public partial class ChatProfileToggle
    {
        private string IdUser { get; set; } = string.Empty;
        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }

        private string imageBase64 = string.Empty;

        private string background = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                Nombre = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
                Apellido = user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value!;
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
                background = $"width: 80px; height: 60px; background-image: url('data:image/jpg;base64,{imageBase64}'); background-size: cover;";
            }
            else
            {
                Iniciales = Nombre.Substring(0, 1).ToUpper() + Apellido.Substring(0, 1).ToUpper();
            }
        }
    }
}
