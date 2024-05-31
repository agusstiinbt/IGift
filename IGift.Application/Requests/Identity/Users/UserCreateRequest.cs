using System.ComponentModel.DataAnnotations;

namespace IGift.Application.Requests.Identity.Users
{
    public class UserCreateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
}

