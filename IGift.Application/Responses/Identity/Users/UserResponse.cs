﻿namespace IGift.Application.Responses.Identity.Users
{
    public class UserResponse
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
    }
}
