using System.ComponentModel.DataAnnotations;

namespace IGift.Application.CQRS.Identity.Users
{
    public class UserLoginRequest
    {
        [EmailAddress(ErrorMessage = "El formato de correo es incorrecto.")]
        [Required(ErrorMessage = "El correo es necesario.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Por favor, ingrese una contraseña.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}