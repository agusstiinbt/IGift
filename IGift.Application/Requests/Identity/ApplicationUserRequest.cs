namespace IGift.Application.Requests.Identity
{
    public class ApplicationUserRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email {  get; set; }
        public required string PhoneNumber { get; set;}

        public ApplicationUserRequest(string userName, string password, string email, string phoneNumber, string firstName, string lastName)
        {
            UserName = userName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
