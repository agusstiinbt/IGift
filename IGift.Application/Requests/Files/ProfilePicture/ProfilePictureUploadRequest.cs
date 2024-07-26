
namespace IGift.Application.Requests.Files.ProfilePicture
{
    public class ProfilePictureUpload
    {
        public required byte[] Data { get; set; }
        public string FileType { get; set; } = string.Empty;
    }
}
