using FluentValidation;

namespace IGift.Application.CQRS.Identity.Users
{
    public class UserCreateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // public string PhoneNumber { get; set; } = string.Empty;
    }

    public class UserCreateValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Debe ingresar un nombre")
                .Length(1, 25);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Debe ingresar un apellido")
                .Length(1, 25);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Debe ingresar un nombre de usuario")
                .Length(1, 30);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
           .NotEmpty().WithMessage("La contraseña es obligatoria.")
           .MinimumLength(7).WithMessage("La contraseña debe tener al menos 7 caracteres.")
           .Matches("[A-Z]").WithMessage("Debe contener al menos una letra mayúscula.")
           .Matches("[a-z]").WithMessage("Debe contener al menos una letra minúscula.")
           .Matches("[0-9]").WithMessage("Debe contener al menos un número.")
           .Matches("[^a-zA-Z0-9]").WithMessage("Debe contener al menos un carácter especial.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Debes confirmar tu contraseña.")
                .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden.");

        }
    }
}

