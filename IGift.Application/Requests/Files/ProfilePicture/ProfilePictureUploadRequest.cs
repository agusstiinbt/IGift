using Microsoft.AspNetCore.Components.Forms;

namespace IGift.Application.Requests.Files.ProfilePicture
{
    public class ProfilePictureUpload
    {
        public IBrowserFile? File { get; set; }
    }
}
