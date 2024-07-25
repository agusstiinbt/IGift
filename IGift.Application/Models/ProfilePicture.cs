namespace IGift.Application.Models
{
    public class ProfilePicture
    {
        public string Id { get; set; }=string.Empty;
        public required string IdUser { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string? FileType { get; set; } = string.Empty;

    }
}
