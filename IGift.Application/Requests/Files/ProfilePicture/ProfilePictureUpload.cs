using IGift.Application.Enums;
using System.Text.Json.Serialization;

namespace IGift.Application.Requests.Files.ProfilePicture
{
    public class ProfilePictureUpload
    {
        [JsonPropertyName("imageDataURL")]//TODO esto lo usamos para que al serializar la propiedad de UploadRequest, las propiedades coincidan todas 
        public string ImageDataURL { get; set; } = string.Empty;

        [JsonPropertyName("uploadRequest")]
        public UploadRequest UploadRequest { get; set; }
    }

}

