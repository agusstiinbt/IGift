namespace IGift.Application.Responses.Files
{
    public class ProfilePictureResponse
    {
        public byte[]? Data { get; set; } = null;
        public DateTime UploadDate { get; set; }
        public string? FileType { get; set; } = string.Empty;
    }
}
