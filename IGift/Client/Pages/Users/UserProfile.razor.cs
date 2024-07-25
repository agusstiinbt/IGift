﻿using Client.Infrastructure.Authentication;
using IGift.Client.Infrastructure.Services.Files;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Buffers.Text;
using System.Security.Claims;

namespace IGift.Client.Pages.Users
{
    public partial class UserProfile
    {
        [Inject] private IProfilePicture _profileService { get; set; }

        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }//TODO implementar en la foto de perfil si esta vacía
        //private string imagenBase64 = "data:image/jpg;base64,/";
        private string imageBase64 = string.Empty;


        private string background = string.Empty;

        IList<IBrowserFile> _files = new List<IBrowserFile>();


        private string Correo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await _profileService.GetByIdAsync("e16eb4a2-96a2-4f57-87b9-ea9dca746ad8");

            imageBase64 = Convert.ToBase64String(result.Data.Data);

            background = $"width: 80px; height: 60px; background-image: url('data:image/jpg;base64,{imageBase64}'); background-size: cover;";
          

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


        private void UploadFiles(IBrowserFile file)
        {
            _files.Add(file);
            //TODO upload the files to the server
        }
    }
}
