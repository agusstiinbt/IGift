using System.ComponentModel.DataAnnotations;

namespace IGift.Application.CQRS.Identity.Password
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
