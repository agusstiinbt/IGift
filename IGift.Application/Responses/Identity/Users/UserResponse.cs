namespace IGift.Application.Responses.Identity.Users
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
    }
}
