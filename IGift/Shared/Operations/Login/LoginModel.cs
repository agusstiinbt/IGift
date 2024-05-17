using System.ComponentModel.DataAnnotations;

namespace IGift.Shared.Operations.Login
{
    public class LoginModel//TODO reemplazar por la clase del Application
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
