using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Shared.Registrar
{
    public partial class RegisterBody
    {
        private UserCreateRequest _user { get; set; } = new UserCreateRequest();
        private void RegistrarUsuario() { }

        [Inject] private IAuthService _authService { get; set; }



        public bool passwordIsShow;
        public InputType PasswordInput = InputType.Password;
        public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        public bool passwordConfirmIsShow;
        public InputType PasswordConfirmInput = InputType.Password;
        public string PasswordInputConfirmIcon = Icons.Material.Filled.VisibilityOff;

        public string verificationAdornmentIcon = Icons.Material.Filled.CheckCircle;
        public Color verificationAdornmentColor = Color.Default;

        protected override Task OnInitializedAsync()
        {
            _user = new UserCreateRequest();
            return base.OnInitializedAsync();
        }

        private async Task HandleRegistration()
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
        void HandleIconPasswordClick()
        {
            IconClick(ref PasswordInput, ref PasswordInputIcon, ref passwordIsShow);
        }

        /// <summary>
        /// Maneja el clic en el ícono de visibilidad de la confirmación de la contraseña.
        /// Cambia el estado de visibilidad de la confirmación de la contraseña y actualiza el ícono correspondiente.
        /// </summary>
        void HandleIconPasswordConfirmClick()
        {
            IconClick(ref PasswordConfirmInput, ref PasswordInputConfirmIcon, ref passwordConfirmIsShow);
        }

        /// <summary>
        /// Cambia el estado de visibilidad de una entrada de contraseña y actualiza el ícono correspondiente.
        /// </summary>
        /// <param name="PasswordInput">La entrada de contraseña a modificar.</param>
        /// <param name="PasswordInputIcon">El ícono de contraseña a actualizar.</param>
        /// <param name="isShow">Un valor booleano que indica si la contraseña es visible o no.</param>
        void IconClick(ref InputType PasswordInput, ref string PasswordInputIcon, ref bool isShow)
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

        /// <summary>
        /// Verifica las credenciales del usuario extranet y actualiza la apariencia visual en función de la validación.
        /// </summary>
        async Task VerifyExtranetUserCredentials()
        {


        }

        public void Dispose()
        {
            ClearViewModelFields();
        }

        private void ClearViewModelFields()
        {

        }

    }
}
