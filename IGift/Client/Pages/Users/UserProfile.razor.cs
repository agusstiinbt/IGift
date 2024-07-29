using Blazored.LocalStorage;
using Client.Infrastructure.Authentication;
using IGift.Application.Requests.Files;
using IGift.Client.Infrastructure.Services.Files;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Claims;
using IGift.Application.Requests.Files.ProfilePicture;

namespace IGift.Client.Pages.Users
{
    public partial class UserProfile
    {
        [Inject] private IProfilePicture _profileService { get; set; }
        [Inject] private ILocalStorageService _localStorage { get; set; }

        private ProfilePictureUpload _picture { get; set; } = new();

        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }

        private string imageBase64 = string.Empty;

        private string background = string.Empty;
        private string Correo { get; set; }

        //Esto se puede usar para más adelante
        IList<IBrowserFile> _files = new List<IBrowserFile>();

        private IBrowserFile _file;

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

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var extension = Path.GetExtension(_file.Name);
                var format = "image/" + extension.Substring(1);
                var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                //Esto de acaá abajo sirve para mostrar en pantalla la foto que acabas de seleccionar
                _picture.ImageDataURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";

                _picture.UploadRequest = new UploadRequest { FileName = "ProfilePicture" + extension, Data = buffer, UploadType = Application.Enums.UploadType.ProfilePicture, Extension = extension };
            }
            await SaveAsync();
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

        private async Task SaveAsync()
        {
            var response = await _profileService.UploadAsync(_picture);
            if (response.Succeeded)
            {
                _snack.Add("Foto de perfil modificada correctamente", MudBlazor.Severity.Success); ;
            }
        }
    }
}