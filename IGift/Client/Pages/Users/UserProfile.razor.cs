using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using IGift.Client.Infrastructure.Services.Files;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Claims;

namespace IGift.Client.Pages.Users
{
    public partial class UserProfile
    {
        [Inject] private IProfilePicture _profileService { get; set; }
        [Inject] private ILocalStorageService _localStorage { get; set; }

        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }

        private string imageBase64 = string.Empty;

        private string background = string.Empty;
        private string Correo { get; set; }

        //Esto se puede usar para más adelante
        IList<IBrowserFile> _files = new List<IBrowserFile>();

        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                Nombre = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
                Apellido = user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value!;
                Correo = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value!;
            }
            await GetProfilePicture();
        }

        private void UploadFiles(IBrowserFile file)
        {
          _profileService.UploadAsync(file);
        }

        private async Task GetProfilePicture()
        {
            var idUser = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.IdUser);

            var result = await _profileService.GetByIdAsync(idUser!);

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