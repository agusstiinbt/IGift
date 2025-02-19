using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Files;
using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components.Forms;

namespace IGift.Client.Pages.Users
{
    public partial class UserProfile
    {
        private ProfilePictureUpload _picture { get; set; } = new();
        private string IdUser { get; set; } = string.Empty;
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
                IdUser = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

            }
            await GetProfilePicture();
        }

        private async Task UploadFiles(IBrowserFile e)
        {
            _file = e;
            if (_file != null)
            {
                var extension = ".png";
                var format = "image/" + extension.Substring(1);
                var imageFile = await e.RequestImageFileAsync(format, 400, 400);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                //Esto de acaá abajo sirve para mostrar en pantalla la foto que acabas de seleccionar
                _picture.ImageDataURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                _picture.IdUser = IdUser;
                _picture.UploadRequest = new UploadRequest { FileName = "ProfilePicture" + extension, Data = buffer, UploadType = Application.Enums.UploadType.ProfilePicture, Extension = extension };
            }
            await SaveAsync();
        }

        private async Task GetProfilePicture()
        {
            var result = await _profileService.GetByIdAsync(IdUser!);

            if (result.Succeeded)
            {
                imageBase64 = Convert.ToBase64String(result.Data.Data);
                background = $"width: 100px; height: 80px; background-image: url('data:image/jpg;base64,{imageBase64}'); background-size: cover;";
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
                await InvokeAsync(() =>
                {
                    _snack.Add("Foto de perfil modificada correctamente", MudBlazor.Severity.Success);
                });
                await Task.Delay(4000);
                _nav.NavigateTo(AppConstants.Routes.UserProfile, true);
            }
            else
            {
                _snack.Add(response.Messages.FirstOrDefault(), MudBlazor.Severity.Error);
            }
        }
    }
}