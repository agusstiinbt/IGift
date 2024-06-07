using System.ComponentModel.DataAnnotations;

namespace IGift.Application.Requests.Identity.Password
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
