using System.ComponentModel.DataAnnotations;

namespace IGift.Application.Requests.Identity
{
    public class UserCreateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at leat {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //TODO hacer un confirmpassword estaba en el blazorhero

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}

