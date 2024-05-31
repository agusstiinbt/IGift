using System.ComponentModel.DataAnnotations;

namespace IGift.Application.Requests.Identity.Users
{
    public class UserLoginRequest
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
