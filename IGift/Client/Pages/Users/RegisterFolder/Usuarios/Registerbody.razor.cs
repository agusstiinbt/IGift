using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages.Users.RegisterFolder.Usuarios
{
    public partial class RegisterBody
    {
        [Inject] private IAuthService _authService { get; set; }

        private UserCreateRequest _user { get; set; } = new UserCreateRequest();
        private bool passwordIsShow;
        private InputType PasswordInput = InputType.Password;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        protected override Task OnInitializedAsync()
        {
            _user = new UserCreateRequest();
            return base.OnInitializedAsync();
        }
        private async Task RegistrarUsuario()
        {
            var result = await _authService.Register(_user);

            if (result.Succeeded)
            {
                _nav.NavigateTo(AppConstants.Routes.Login + "/true");
            }
            else
            {
                _snack.Add(result.Messages.First());
            }
        }

        /// <summary>
        /// Maneja el clic en el ícono de visibilidad de la contraseña.
        /// Cambia el estado de visibilidad de la contraseña y actualiza el ícono correspondiente.
        /// </summary>
        private void HandleIconPasswordClick()
        {
            IconClick(ref PasswordInput, ref PasswordInputIcon, ref passwordIsShow);
        }

        /// <summary>
        /// Cambia el estado de visibilidad de una entrada de contraseña y actualiza el ícono correspondiente.
        /// </summary>
        /// <param name="PasswordInput">La entrada de contraseña a modificar.</param>
        /// <param name="PasswordInputIcon">El ícono de contraseña a actualizar.</param>
        /// <param name="isShow">Un valor booleano que indica si la contraseña es visible o no.</param>
        private void IconClick(ref InputType PasswordInput, ref string PasswordInputIcon, ref bool isShow)
        {
            if (isShow)
            {
                // Si la contraseña es visible, ocultarla y cambiar el ícono a "Ocultar".
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                // Si la contraseña está oculta, mostrarla y cambiar el ícono a "Mostrar".
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }
    }
}
