namespace IGift.Application.Responses.Identity.Users
{
    public class UserResponse
    {
        public required string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ProfilePictureDataUrl { get; set; }
    }
}
