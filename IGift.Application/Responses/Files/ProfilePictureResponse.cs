using System.Runtime.InteropServices;

namespace IGift.Application.Responses.Files
{
    public class ProfilePictureResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public required byte[] Data { get; set; }
    }
}
