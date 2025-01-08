using System.Text.Json.Serialization;

namespace IGift.Application.CQRS.Files.ProfilePicture
{
    /// <summary>
    /// Clase que se encarga de guardar información al respecto sobre cómo subir una foto de perfil
    /// </summary>
    public class ProfilePictureUpload
    {
        [JsonPropertyName("ImageDataURL")]// Esto lo usamos para que al serializar la propiedad de UploadRequest, las propiedades coincidan todas 
        public string ImageDataURL { get; set; } = string.Empty;

        [JsonPropertyName("uploadRequest")]
        public UploadRequest UploadRequest { get; set; }

        [JsonPropertyName("IdUser")]
        public string IdUser { get; set; }
    }

}

