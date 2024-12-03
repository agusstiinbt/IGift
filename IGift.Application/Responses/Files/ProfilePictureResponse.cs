namespace IGift.Application.Responses.Files
{
    public class ProfilePictureResponse
    {
        public required byte[] Data { get; set; }
        public DateTime UploadDate { get; set; }//TODO esto se usa?
        public string? FileType { get; set; } = string.Empty;

    }
}
